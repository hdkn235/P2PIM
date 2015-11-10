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
        private SysConfig config;

        public SysConfig Config
        {
            get { return config; }
            set { config = value; }
        }

        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SysConfig sys = this.gridConfig.DataContext as SysConfig;
            ConfigHelper.SetInstanceToConfig<SysConfig>(sys);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.gridConfig.DataContext = Config;
        }
    }
}
