using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P2PIM.Model
{
    public enum UserLoginState
    { 
        Login,
        Logout,
        Accept,
        Talk
    }
    public class UserDTO
    {
        public UserLoginState LoginState { get; set; }

        public User LoginUser { get; set; }

        public int ServerTcpPort { get; set; }

        public string SendTime { get; set; }

        public string SendContent { get; set; }
    }
}
