using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Reflection;
using KanColleNote.Model;

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
        public static JObject jsonTable { set; get; }
        //public static Hashtable hashTable;
        public static Task task;
        public static string filePath;

        static SpeedConfig()
        {
            //固定会做初始化 所以这里不
            //jsonTable = new JObject();
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
                jsonTable = JObject.Parse(File.ReadAllText(filePath));//JsonConvert.DeserializeObject<Hashtable>(File.ReadAllText(filePath));
                //Set(ConfigName.CONFIG_PROXY_OPEN, true);
                CheckValue();
                Save();
            }
            else
            {
                //初始化变量
                jsonTable = new JObject();
                CheckValue();
                Save();
            }
            
        }

        public static void CheckValue()
        {
            var obj = new ConfigName();
            var type = obj.GetType();
            var p = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            //foreach (var item in p)
            //{
            //    Console.WriteLine("Name: {0}", item.Name);
            //}

            foreach (FieldInfo field in type.GetFields())
            {
                Console.WriteLine("Field: {0}, Value:{1}", field.Name, field.GetValue(obj));

                if (jsonTable.Property((string)field.GetValue(obj)) != null)
                {
                    continue; //有这个值了 不再初始化
                }

                //对一些值进行初始化
                if (field.Name.Equals("CONFIG_PROXY_OPEN"))
                {
                    jsonTable[field.GetValue(obj)] = false;
                }
                else if (field.Name.Equals("CONFIG_PROXY_IP"))
                {
                    jsonTable[field.GetValue(obj)] = "127.0.0.1";
                }
                else if (field.Name.Equals("CONFIG_PROXY_PORT"))
                {
                    jsonTable[field.GetValue(obj)] = 1080;
                }
                else if (field.Name.Equals("CONFIG_PROXY_SELFPORT"))
                {
                    jsonTable[field.GetValue(obj)] = 37180;
                }
                else
                {
                    jsonTable[field.GetValue(obj)] = "";
                }

            }
        }

        public static void Set(string key, object value)
        {
            jsonTable[key] = JToken.FromObject(value);
            //System.IO.File.WriteAllText(App.AppDataPath + @"\config.json", JsonConvert.SerializeObject(hashTable));
            Save();

        }


        /// <summary>
        /// 存储配置
        /// </summary>
        public static void Save()
        {
            if (task == null || task.Status != TaskStatus.Running)
            {
                task = Task.Factory.StartNew(() =>
                {
                    //更新数据任务
                    Thread.Sleep(1000);
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(jsonTable, Formatting.Indented));
                    GlobalNotification.Default.Post(NotificationType.kConfigUpdate, null);
                });
                //task.Start();
            }
            //NekoProxy.
            
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public static T Get<T>(string key, T def)
        {
            if (jsonTable.SelectToken(key) != null)
            {
                if (jsonTable.SelectToken(key).Type == JTokenType.Null)
                {
                    //null不能转T
                }
                return (T)jsonTable[key].Value<T>();
            }
            return def;
            
        }

    }
}
