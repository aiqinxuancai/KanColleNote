using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.Threading;
using System.IO;

namespace KanColleNote.Base
{
    public class ConfigName
    {
        public const string CONFIG_PROXY_OPEN = "ProxyOpen";
        public const string CONFIG_PROXY_IP = "ProxyIP";
        public const string CONFIG_PROXY_PORT = "ProxyPort";
        public const string CONFIG_PROXY_SELFPORT = "ProxySelfPort";
    }


    /// <summary>
    /// 存储配置
    /// </summary>
    public class SpeedConfig
    {
        public static Hashtable hashTable;
        public static Task task;
        public static string filePath;

        static SpeedConfig()
        {
            hashTable = new Hashtable();
            //task = new Task(() =>{
            //    //更新数据任务
            //    Thread.Sleep(1000);
            //    System.IO.File.WriteAllText(App.TKBAppPath + @"\config.json", JsonConvert.SerializeObject(hashTable));
            //});
        }

        public static void Load(string file)
        {
            filePath = file;

            if (File.Exists(filePath)) //App.TKBAppPath + @"\config.json"
            {
                hashTable = JsonConvert.DeserializeObject<Hashtable>(File.ReadAllText(filePath));
            }
            else
            {
                //初始化变量
            }
            
        }

        public static void Set(string key, object value)
        {
            if (hashTable.ContainsKey(key))
            {
                hashTable[key] = value;
            } else
            {
                hashTable.Add(key, value);
            }
                
            //System.IO.File.WriteAllText(App.AppDataPath + @"\config.json", JsonConvert.SerializeObject(hashTable));
            
            if (task == null || task.Status != TaskStatus.Running)
            {
                task = Task.Factory.StartNew(() =>
                {
                    //更新数据任务
                    Thread.Sleep(1000);
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(hashTable, Formatting.Indented));
                });
                //task.Start();
            }
        }

        public static T Get<T>(string key, T def)
        {
            if (hashTable.ContainsKey(key))
            {
                return (T)hashTable[key];
            }
            return def;
            
        }

    }
}
