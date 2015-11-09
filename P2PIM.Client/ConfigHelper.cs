using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Configuration;

namespace P2PIM.Client
{
    public class ConfigHelper
    {
        public static T GetInstanceFromConfig<T>()
        {
            T t = System.Activator.CreateInstance<T>();
            Type type = t.GetType();
            foreach (PropertyInfo pt in type.GetProperties())
            {
                object value = ConfigurationManager.AppSettings[pt.Name];
                if (value != null)
                {
                    if (pt.PropertyType == typeof(int))
                    {
                        pt.SetValue(t, Convert.ToInt32(value), null);
                    }
                    else
                    {
                        pt.SetValue(t, value, null);
                    }
                }
            }
            return t;
        }

        public static void SetInstanceToConfig<T>(T t)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Type type = t.GetType();
            foreach (PropertyInfo pt in type.GetProperties())
            {
                string name = pt.Name;
                string value = pt.GetValue(t, null) == null ? string.Empty : pt.GetValue(t, null).ToString();
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
