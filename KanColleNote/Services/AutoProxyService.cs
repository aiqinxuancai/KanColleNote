using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Security.Principal;

namespace KanColleNote.Services
{
    /// <summary>
    /// 自动配置代理，支持acgp和岛风Go及ss
    /// </summary>
    class AutoProxyService
    {
        public bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        const string ACGPOWER = "ACGPower";
        const string SHIMAKAZEGO = "ShimakazeGo";
        const string SHADOWSOCKS = "Shadowsocks";

        public Task<string> AutoSetting()
        {
            return Task.Run(() =>
            {
                Process process = new Process();
                ProcessStartInfo shex = new ProcessStartInfo();
                shex.WindowStyle = ProcessWindowStyle.Hidden;
                shex.Arguments = "";
                shex.Verb = "runas";
                shex.FileName = Directory.GetCurrentDirectory() + @"\GetProxyProcess.exe"; ;
                shex.WorkingDirectory = Directory.GetCurrentDirectory();

                process.StartInfo = shex;

                var IsAdmin = IsAdministrator();

                if (File.Exists(shex.FileName))
                {
                    process.Start();
                    process.WaitForExit();

                    var proxyStatus = Directory.GetCurrentDirectory() + @"\ProxyStatus.json";

                    if (File.Exists(proxyStatus))
                    {
                        var json = File.ReadAllText(proxyStatus);
                        File.Delete(proxyStatus);
                        JObject root = JObject.Parse(json);

                        if (root.Property(ACGPOWER) != null)
                        {
                            var port = GetACGPowerProxy(root[ACGPOWER].Value<string>());
                            JObject ret = new JObject();
                            ret["type"] = ACGPOWER;
                            ret["port"] = port;
                            return ret.ToString(Newtonsoft.Json.Formatting.None);
                        }
                        else if (root.Property(SHIMAKAZEGO) != null)
                        {
                            var port = GetShimakazeGoProxy(root[SHIMAKAZEGO].Value<string>());
                            JObject ret = new JObject();
                            ret["type"] = SHIMAKAZEGO;
                            ret["port"] = port;
                            return ret.ToString(Newtonsoft.Json.Formatting.None);
                        }
                        else if (root.Property(SHADOWSOCKS) != null)
                        {
                            var port = GetShadowsocksProxy(root[SHADOWSOCKS].Value<string>());
                            JObject ret = new JObject();
                            ret["type"] = SHADOWSOCKS;
                            ret["port"] = port;
                            return ret.ToString(Newtonsoft.Json.Formatting.None);
                        }

                    }
                }

                return string.Empty;
            });
        }

        public string GetACGPowerProxy(string path)
        {
            var configPath = Directory.GetParent(path).FullName + @"\appsettings.config";
            if (File.Exists(configPath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configPath);
                XmlElement rootElement = doc.DocumentElement;
                var port = rootElement["Port"].InnerText;
                return port;
            }
            return "";
        }

        public string GetShimakazeGoProxy(string path)
        {
            var configPath = Directory.GetParent(path).FullName + @"\config.xml";
            if (File.Exists(configPath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configPath);
                XmlElement rootElement = doc.DocumentElement;
                var port = rootElement["LocalProxyPort"].InnerText;
                return port;
            }
            return "";
        }

        public string GetShadowsocksProxy(string path)
        {
            var configPath = Directory.GetParent(path).FullName + @"\gui-config.json";
            if (File.Exists(configPath))
            {
                JObject root = JObject.Parse(File.ReadAllText(configPath));
                if (root.Property("localPort") != null)
                {
                    var port = root["localPort"].Value<string>();
                    return port;
                }
                
            }
            return "";
        }
    }
}
