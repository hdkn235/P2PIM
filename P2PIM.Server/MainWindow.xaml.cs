using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using P2PIM.Model;
using Newtonsoft.Json;
using P2PIM.Common;

namespace P2PIM.Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPAddress serverIP;
        private IPEndPoint serverIPEndPoint;
        private UdpClient receiveUdpClient;
        private List<User> userList = new List<User>();
        private UdpClient sendUdpClient;
        private int tcpPort;
        private TcpListener tcpListener;
        private string userListstring;
        private NetworkStream networkStream;
        private BinaryWriter binaryWriter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            serverIP = IPAddress.Parse(txtServerIP.Text);
            serverIPEndPoint = new IPEndPoint(serverIP, int.Parse(txtServerPort.Text));
            receiveUdpClient = new UdpClient(serverIPEndPoint);
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;

            //随机指定TCP监听端口
            Random random = new Random();
            tcpPort = random.Next(int.Parse(txtServerPort.Text) + 1, 65536);
            tcpListener = new TcpListener(serverIP, tcpPort);
            tcpListener.Start();

            Thread listenThread = new Thread(ListenClientConnect);
            listenThread.Start();
            AddMsgToLog(string.Format("服务器线程{0}启动,TCP监听端口{1},UDP监听端口{2}", serverIPEndPoint, tcpPort,txtServerPort.Text));
        }

        /// <summary>
        /// 监听指定端口 接收客户端发来的消息
        /// </summary>
        private void ReceiveMessage()
        {
            IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] receiveBytes = receiveUdpClient.Receive(ref remoteIPEndPoint);
                    string message = Encoding.Unicode.GetString(receiveBytes, 0, receiveBytes.Length);

                    AddMsgToLog(string.Format("{0}:{1}", remoteIPEndPoint, message));

                    //string[] splitstring = message.Split(',');
                    //string[] splitsubstring = splitstring[2].Split(':');
                    //IPEndPoint clientIPEndPoint = new IPEndPoint(IPAddress.Parse(splitsubstring[0]), int.Parse(splitsubstring[1]));

                    UserDTO dto = JsonConvert.DeserializeObject<UserDTO>(message,JsonHelper.GetIPJsonSettings());
                    switch (dto.LoginState)
                    {
                        case UserLoginState.Login:
                            //User user = new User(splitstring[1], clientIPEndPoint);
                            User user = dto.LoginUser;
                            userList.Add(user);
                            AddMsgToLog(string.Format("用户{0}({1})加入", user.Name, user.IPAndPort));
                            //string sendMsg = "accept," + tcpPort.ToString();
                            string sendMsg = JsonConvert.SerializeObject(new UserDTO
                            {
                                LoginState = UserLoginState.Accept,
                                ServerTcpPort = tcpPort
                            });
                            SendToClient(user, sendMsg);
                            AddMsgToLog(string.Format("向{0}({1})发出：[{2}]", user.Name, user.IPAndPort, sendMsg));
                            foreach (User otherUser in userList)
                            {
                                if (otherUser.ID != user.ID)
                                {
                                    SendToClient(otherUser, message);
                                }
                            }
                            AddMsgToLog(string.Format("广播：[{0}]", message));
                            break;
                        case UserLoginState.Logout:
                            for (int i = 0; i < userList.Count; i++)
                            {
                                if (userList[i].ID == dto.LoginUser.ID)
                                {
                                    AddMsgToLog(string.Format("用户{0}({1})退出", userList[i].Name, userList[i].IPAndPort));
                                    // 移除用户
                                    userList.RemoveAt(i);
                                }
                            }
                            for (int i = 0; i < userList.Count; i++)
                            {
                                // 广播注销消息
                                SendToClient(userList[i], message);
                            }
                            AddMsgToLog(string.Format("广播:[{0}]", message));
                            break;
                    }
                }
                catch
                {
                    break;
                }
            }
            AddMsgToLog(string.Format("服务线程{0}终止", serverIPEndPoint));
        }

        private void AddMsgToLog(string msg)
        {
            Action<TextBox, string> updateTbAction = new Action<TextBox, string>(UpdateTb);
            txtLog.Dispatcher.BeginInvoke(updateTbAction, txtLog, msg);
        }

        private void UpdateTb(TextBox tb, string msg)
        {
            tb.Text += msg + "\n";
        }

        private void SendToClient(User user, string msg)
        {
            sendUdpClient = new UdpClient(0);
            byte[] sendBytes = Encoding.Unicode.GetBytes(msg);
            IPEndPoint remoteIPEndPoint = user.IPAndPort;
            sendUdpClient.Send(sendBytes, sendBytes.Length, remoteIPEndPoint);
            sendUdpClient.Close();
        }

        private void ListenClientConnect()
        {
            TcpClient tcpClient = null;
            while (true)
            {
                try
                {
                    tcpClient = tcpListener.AcceptTcpClient();
                    AddMsgToLog(string.Format("接受客户端{0}的TCP请求", tcpClient.Client.RemoteEndPoint));
                }
                catch
                {
                    AddMsgToLog(string.Format("监听线程({0}:{1})", serverIP, tcpPort));
                    break;
                }

                Thread sendDataThread = new Thread(SendData);
                sendDataThread.Start(tcpClient);
            }
        }

        private void SendData(object userClient)
        {
            TcpClient newUserClient = (TcpClient)userClient;
            //userListstring = null;
            //for (int i = 0; i < userList.Count; i++)
            //{
            //    userListstring += userList[i].Name + ","
            //        + userList[i].IPAndPort.ToString() + ";";
            //}
            //userListstring += "end";
            userListstring = JsonConvert.SerializeObject(userList,JsonHelper.GetIPJsonSettings());

            networkStream = newUserClient.GetStream();
            binaryWriter = new BinaryWriter(networkStream);
            binaryWriter.Write(userListstring);
            binaryWriter.Flush();
            AddMsgToLog(string.Format("向{0}发送[{1}]", newUserClient.Client.RemoteEndPoint, userListstring));
            binaryWriter.Close();
            newUserClient.Close();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            receiveUdpClient.Close();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            receiveUdpClient.Close();
            tcpListener.Stop();
        }
    }
}
