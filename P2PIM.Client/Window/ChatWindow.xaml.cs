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
using System.Windows.Shapes;
using P2PIM.Model;
using System.Net.Sockets;
using System.Threading;
using P2PIM.Common;
using Newtonsoft.Json;

namespace P2PIM.Client
{
    /// <summary>
    /// ChatWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChatWindow : Window
    {
        private UdpClient sendUdpClient;

        public User SendUser { get; set; }

        public User ReceiveUser { get; set; }

        public ChatWindow()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            sendUdpClient = new UdpClient();
            Thread sendTread = new Thread(SendMessage);
            UserDTO dto = new UserDTO
            {
                LoginState = UserLoginState.Talk,
                SendTime = DateTime.Now.ToLongTimeString(),
                LoginUser = SendUser,
                SendContent = txtSend.Text
            };
            sendTread.Start(JsonConvert.SerializeObject(dto, JsonHelper.GetIPJsonSettings()));
            txtTalk.AppendText(SendUser.Name + "    " + DateTime.Now.ToLongTimeString() + Environment.NewLine + txtSend.Text);
            txtTalk.AppendText(Environment.NewLine);
            txtTalk.ScrollToEnd();
            txtSend.Text = "";
            txtSend.Focus();
        }

        private void SendMessage(object obj)
        {
            byte[] sendBytes = Encoding.Unicode.GetBytes(obj.ToString());
            sendUdpClient.Send(sendBytes, sendBytes.Length, ReceiveUser.IPAndPort);
            sendUdpClient.Close();
        }

        public void ShowTalkInfo(UserDTO dto)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                txtTalk.AppendText(dto.LoginUser.Name + "    " + dto.SendTime + Environment.NewLine + dto.SendContent);
                txtTalk.AppendText(Environment.NewLine);
                txtTalk.ScrollToEnd();
            }));
        }
    }
}
