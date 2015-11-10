using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.ComponentModel;

namespace P2PIM.Client
{
    [Serializable]
    public sealed class SysConfig : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string userHeadPath;
        private string userName;
        private string userAutograph;

        /// <summary>
        /// 头像的路径
        /// </summary>
        public String UserHeadPath
        {
            set
            {
                userHeadPath = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("UserHeadPath"));
                }
            }
            get
            {
                return userHeadPath;
            }
        }
        /// <summary>
        /// 好友名字
        /// </summary>
        public String UserName
        {
            set
            {
                userName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("UserName"));
                }
            }
            get
            {
                return userName;
            }
        }
        /// <summary>
        /// 签名
        /// </summary>
        public String UserAutograph
        {
            set
            {
                userAutograph = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("UserAutograph"));
                }
            }
            get
            {
                return userAutograph;
            }
        }
        /// <summary>
        /// 本机IP
        /// </summary>
        public string LocalIP { get; set; }
        /// <summary>
        /// 本机Port
        /// </summary>
        public int LocalPort { get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP { get; set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort { get; set; }

        public SysConfig()
        {
            UserHeadPath = "/Resources/Heads/h1.png";
        }

    }
}
