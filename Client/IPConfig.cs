using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;

namespace P2PIM.Client
{
    [Serializable]
    public sealed class IPConfig
    {
        private static IPConfig ipConfig;

        private static object obj = new object();

        private IPConfig()
        {

        }

        public static IPConfig GetInstance()
        {
            if (ipConfig == null)
            {
                lock (obj)
                {
                    if (ipConfig == null)
                    {
                        ipConfig = new IPConfig();
                        Type t = ipConfig.GetType();
                        foreach (PropertyInfo pt in t.GetProperties())
                        {
                            object value = ConfigurationManager.AppSettings[pt.Name];
                            if (value != null)
                            {
                                if (pt.PropertyType == typeof(int))
                                {
                                    pt.SetValue(ipConfig, Convert.ToInt32(value), null);
                                }
                                else
                                {
                                    pt.SetValue(ipConfig, value, null);
                                }
                            }
                        }
                    }
                }
            }
            return ipConfig;
        }

        public string LocalIP { get; set; }

        public int LocalPort { get; set; }

        public string ServerIP { get; set; }

        public int ServerPort { get; set; }

        public string UserName { get; set; }

        public void Save()
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Type t = ipConfig.GetType();
            foreach (PropertyInfo pt in t.GetProperties())
            {
                string name = pt.Name;
                string value = pt.GetValue(ipConfig, null) == null ? string.Empty : pt.GetValue(ipConfig, null).ToString();
                if (ConfigurationManager.AppSettings[name] == null)
                {
                    cfa.AppSettings.Settings.Add(name, value);
                }
                else
                {
                    cfa.AppSettings.Settings[name].Value = value;
                }
            }
            cfa.Save();
            ConfigurationManager.RefreshSection("AppSettings");
        }

    }
}
