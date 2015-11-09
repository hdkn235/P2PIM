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
using System.Collections.ObjectModel;
using P2PIM.Model;
using Newtonsoft.Json;
using P2PIM.Common;

namespace P2PIM.Client
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

        //系统配置
        private SysConfig sysConfig;
        //登录用户
        private User loginUser;
        //用户列表
        private ObservableCollection<User> users = new ObservableCollection<User>();

        public MainWindow()
        {
            InitializeComponent();

            this.tvUsers.ItemsSource = users;

            sysConfig = ConfigHelper.GetInstanceFromConfig<SysConfig>();
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindow cw = new ConfigWindow();
            cw.ShowDialog();
        }

        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            IPAddress clientIP = IPAddress.Parse(sysConfig.LocalIP);
            clientIPEndPoint = new IPEndPoint(clientIP, sysConfig.LocalPort);
            loginUser = new User(sysConfig.UserName, clientIPEndPoint);

            receiveUdpClient = new UdpClient(clientIPEndPoint);
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();

            sendUdpClient = new UdpClient(0);
            Thread sendThread = new Thread(SendMessage);
            UserDTO dto = new UserDTO
            {
                LoginState = UserLoginState.Login,
                LoginUser = loginUser
            };
            sendThread.Start(JsonConvert.SerializeObject(dto, JsonHelper.GetIPJsonSettings()));

            this.btnRefresh.IsEnabled = false;
            this.btnLogout.IsEnabled = true;
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
                    UserDTO dto = JsonConvert.DeserializeObject<UserDTO>(message, JsonHelper.GetIPJsonSettings());
                    switch (dto.LoginState)
                    {
                        case UserLoginState.Accept:
                            try
                            {
                                tcpClient = new TcpClient();
                                tcpClient.Connect(remoteIPEndPoint.Address, dto.ServerTcpPort);
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
                        case UserLoginState.Login:
                            AddUserToTV(dto.LoginUser);
                            break;
                        case UserLoginState.Logout:
                            RemoveUserFromTV(dto.LoginUser);
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
        /// 发送请求
        /// </summary>
        /// <param name="obj"></param>
        private void SendMessage(object obj)
        {
            string message = obj.ToString();
            byte[] sendBytes = Encoding.Unicode.GetBytes(message);
            IPAddress remoteIP = IPAddress.Parse(sysConfig.ServerIP);
            IPEndPoint remoteIPEndPoint = new IPEndPoint(remoteIP, sysConfig.ServerPort);
            sendUdpClient.Send(sendBytes, sendBytes.Length, remoteIPEndPoint);
            sendUdpClient.Close();
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
                    List<User> newUsers = JsonConvert.DeserializeObject<List<User>>(userListstring, JsonHelper.GetIPJsonSettings());
                    foreach (User user in newUsers)
                    {
                        AddUserToTV(user);
                    }
                    binaryReader.Close();
                    tcpClient.Close();
                    break;
                }
                catch
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 添加用户到列表
        /// </summary>
        /// <param name="userStr"></param>
        private void AddUserToTV(User user)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                users.Add(user);
            }));
        }

        /// <summary>
        /// 移除用户
        /// </summary>
        /// <param name="userName"></param>
        private void RemoveUserFromTV(User user)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                for (int i = users.Count - 1; i >= 0; i--)
                {
                    if (users[i].ID == user.ID)
                    {
                        users.RemoveAt(i);
                    }
                }
            }));
        }

        /// <summary>
        /// 注销按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            Logout();
            this.users.Clear();
            this.btnRefresh.IsEnabled = true;
            this.btnLogout.IsEnabled = false;
        }

        /// <summary>
        /// 用户退出
        /// </summary>
        private void Logout()
        {
            // 匿名发送
            sendUdpClient = new UdpClient();
            //启动发送线程
            Thread sendThread = new Thread(SendMessage);
            UserDTO dto = new UserDTO
            {
                LoginState = UserLoginState.Logout,
                LoginUser = loginUser
            };
            sendThread.Start(JsonConvert.SerializeObject(dto, JsonHelper.GetIPJsonSettings()));

            if (receiveUdpClient != null)
            {
                receiveUdpClient.Close();
            }
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Logout();
        }
    }
}
