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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using P2PIM.Model;

namespace P2PIM.Client
{
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void LoadConfig()
        {
            this.gridSysConfig.DataContext = ConfigHelper.GetInstanceFromConfig<SysConfig>();
            this.gridUser.DataContext = ConfigHelper.GetInstanceFromConfig<User>();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SysConfig sys = this.gridSysConfig.DataContext as SysConfig;
            ConfigHelper.SetInstanceToConfig<SysConfig>(sys);

            User user = this.gridSysConfig.DataContext as User;
            ConfigHelper.SetInstanceToConfig<User>(user);
        }
    }
}
