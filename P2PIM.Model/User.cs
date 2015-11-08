using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace P2PIM.Model
{
    public class User
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 头像的路径
        /// </summary>
        public String HeadPath { set; get; }
        /// <summary>
        /// 好友名字
        /// </summary>
        public String Name { set; get; }
        /// <summary>
        /// 签名
        /// </summary>
        public String Autograph { set; get; }
        /// <summary>
        /// 用户IP和端口
        /// </summary>
        public IPEndPoint IPAndPort { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public User()
        {
            ID = Guid.NewGuid().ToString();
            HeadPath = "/Resources/Heads/h1.png";
            Autograph = "非一般的感觉";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ipEndPoint"></param>
        public User(string name, IPEndPoint ipEndPoint)
            : this()
        {
            Name = name;
            IPAndPort = ipEndPoint;
        }
    }
}
