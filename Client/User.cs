using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P2PIM.Client
{
    class User
    {
        //头像的路径
        public String HeadPath { set; get; }
        //好友名字
        public String Name { set; get; }
        //签名
        public String Autograph { set; get; }
        /// <summary>
        /// 用户IP和端口
        /// </summary>
        public string IPAndPort { get; set; }
    }
}
