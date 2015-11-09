using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace P2PIM.Client
{
    [Serializable]
    public sealed class SysConfig
    {
        public string LocalIP { get; set; }

        public int LocalPort { get; set; }

        public string ServerIP { get; set; }

        public int ServerPort { get; set; }

        public string UserName { get; set; }
    }
}
