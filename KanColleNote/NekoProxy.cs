using KanColleNote.Base;
using KanColleNote.Model;
using Nekoxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanColleNote
{
    class NekoProxy
    {



        public static void InitNekoxy() //使用Nekoxy库来处理封包
        {
            ReLoadNekoxy();
            HttpProxy.AfterReadRequestHeaders += HttpProxy_AfterReadRequestHeaders;
            HttpProxy.AfterReadResponseHeaders += HttpProxy_AfterReadResponseHeaders;
            HttpProxy.AfterSessionComplete += HttpProxy_AfterSessionComplete;

            GlobalNotification.Default.Register(NotificationType.kConfigUpdate, typeof(NekoProxy), OnConfigUpdate);
        }

        /// <summary>
        /// 接收修改配置项的消息
        /// </summary>
        /// <param name="msg"></param>
        public static void OnConfigUpdate(GlobalNotificationMessage msg)
        {
            ReLoadNekoxy();
        }

        public static void ReLoadNekoxy()
        {
            HttpProxy.Shutdown();
            //
            bool open = SpeedConfig.Get<bool>(ConfigName.CONFIG_PROXY_OPEN, false);
            string ip = SpeedConfig.Get<string>(ConfigName.CONFIG_PROXY_IP, "127.0.0.1");
            int port = SpeedConfig.Get<int>(ConfigName.CONFIG_PROXY_PORT, 8123);
            int selfPort = SpeedConfig.Get<int>(ConfigName.CONFIG_PROXY_SELFPORT, 37180);

            if (open)
            {
                HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SpecificProxy, ip, port);
            }
            else
            {
                HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SpecificProxy);
            }

 
            //判断端口是否被占用？

            HttpProxy.Startup(selfPort, false, false);


            //这里要做一下循环的Try，以免中转端口被占用

            //for (int i = 0; i < 10; i++)
            //{
            //    var port = 37160 + i;
            //    if (NetHelper.PortInUse(port) == false)
            //    {
            //        m_port = port;
                    
            //        IEProxy.SetProxy($"127.0.0.1:{port}");
            //        break;
            //    }
            //}
        }

        private static void HttpProxy_AfterSessionComplete(Session obj)
        {
            //Task.Run(() => Debug.WriteLine(obj));
            //执行封包操作
            HttpProxyToOldProc(obj);
        }

        private static void HttpProxy_AfterReadResponseHeaders(HttpResponse obj)
        {
            //Task.Run(() => Console.WriteLine(obj));
        }

        private static void HttpProxy_AfterReadRequestHeaders(HttpRequest obj)
        {
            //Task.Run(() => Console.WriteLine(obj));
        }

        private static void HttpProxyToOldProc(Session obj)
        {
            if (obj.Request.RequestLine.Method == "POST")
            {
                if (obj.Response.BodyAsString.StartsWith("svdata"))
                {
                    //数据处理分发
                    //obj.Response.
                    //KanColleNoteCore.KanColleNoteCore.RecvPack(obj);
                    DataRoute.RecvPack(obj);
                }
                //jsonString = obj.Response.BodyAsString
                //obj.Request
                //JsonProc(obj.Request.PathAndQuery.Replace("/", "_"), obj.Response.BodyAsString, obj.Request.BodyAsString, "");
            }
        }
    }
}
