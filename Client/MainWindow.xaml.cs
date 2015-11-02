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

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //变量
        private IPEndPoint clientIPEndPoint;
        private UdpClient receiveUdpClient;
        private UdpClient sendUdpClient;
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private BinaryReader binaryReader;
        private string userListstring;

        private IPConfig ipConfig = IPConfig.GetInstance();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow cw = new ConfigWindow();
            cw.ShowDialog();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            IPAddress clientIP = IPAddress.Parse(ipConfig.LocalIP);
            clientIPEndPoint = new IPEndPoint(clientIP, ipConfig.LocalPort);

            receiveUdpClient = new UdpClient(clientIPEndPoint);
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();

            sendUdpClient = new UdpClient(0);
            Thread sendThread = new Thread(SendMessage);
            sendThread.Start(string.Format("login,{0},{1}", ipConfig.UserName, clientIPEndPoint));
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="obj"></param>
        private void SendMessage(object obj)
        {
            string message = obj.ToString();
            byte[] sendBytes = Encoding.Unicode.GetBytes(message);
            IPAddress remoteIP = IPAddress.Parse(ipConfig.ServerIP);
            IPEndPoint remoteIPEndPoint = new IPEndPoint(remoteIP, ipConfig.ServerPort);
            sendUdpClient.Send(sendBytes, sendBytes.Length, remoteIPEndPoint);
            sendUdpClient.Close();
        }

        /// <summary>
        /// 客户端接受服务器的回应消息
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
                    string[] splitString = message.Split(',');
                    switch (splitString[0])
                    {
                        case "Accept":
                            try
                            {
                                tcpClient = new TcpClient();
                                tcpClient.Connect(remoteIPEndPoint.Address, int.Parse(splitString[1]));
                                if (tcpClient != null)
                                {
                                    networkStream = tcpClient.GetStream();
                                    binaryReader = new BinaryReader(networkStream);
                                }
                            }
                            catch
                            {
                                MessageBox.Show("连接失败！", "异常");
                            }
                            Thread getUserListThread = new Thread(GetUserList);
                            getUserListThread.Start();
                            break;
                        case "login":
                            string userItem = splitString[1] + "," + splitString[2];
                            break;
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        private void GetUserList()
        {
            while (true)
            {
                userListstring = null;
                try
                {
                    userListstring = binaryReader.ReadString();
                    if (userListstring.EndsWith("end"))
                    {
                        string[] splitstring = userListstring.Split(';');
                        for (int i = 0; i < splitstring.Length - 1; i++)
                        {
                            AddUserToTreeView(splitstring[i]);
                        }

                        binaryReader.Close();
                        tcpClient.Close();
                        break;
                    }
                }
                catch 
                {
                    break;
                }
            }
        }

        private void AddUserToTreeView(string userInfo)
        { 
            
        }
    }
}
