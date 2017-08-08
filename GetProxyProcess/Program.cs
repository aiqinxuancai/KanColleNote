using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetProxyProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
	            JObject root = new JObject();
	            var acg = Process.GetProcessesByName("ACGPower");
	            var go = Process.GetProcessesByName("ShimakazeGo");
	            var ss = Process.GetProcessesByName("Shadowsocks");
	            if (acg.Length > 0)
	            {
	                try
	                {
	                	root["ACGPower"] = acg[0].MainModule.FileName;
	                }
	                catch (System.Exception ex)
	                {
	                	
	                }
	            }
	            if (go.Length > 0)
	            {
	                try
	                {
	                	root["ShimakazeGo"] = go[0].MainModule.FileName;
	                }
	                catch (System.Exception ex)
	                {
	                	
	                }
	            }
	            if (ss.Length > 0)
	            {
	                try
	                {
	                	root["Shadowsocks"] = ss[0].MainModule.FileName;
	                }
	                catch (System.Exception ex)
	                {
	                	
	                }
	            }
	
	            File.WriteAllText(Directory.GetCurrentDirectory() + @"\ProxyStatus.json", root.ToString(Formatting.Indented));
            }
            catch (System.Exception ex)
            {

                Debug.WriteLine(ex);
            }



        }
    }
}
