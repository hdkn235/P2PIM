using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Server
{
    // 用户类
    public class User
    {
        // 私有字段
        private string username;
        private IPEndPoint userIPEndPoint;

        //构造函数
        public User(string name, IPEndPoint ipEndPoint)
        {
            username = name;
            userIPEndPoint = ipEndPoint;
        }

        // 公共方法
        public string GetName()
        {
            return username;
        }

        public IPEndPoint GetIPEndPoint()
        {
            return userIPEndPoint;
        }
    }
}
