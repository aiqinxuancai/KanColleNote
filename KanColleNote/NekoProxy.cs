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
        }
        public static void ReLoadNekoxy()
        {
            HttpProxy.Shutdown();
            //HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SpecificProxy);
            HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SpecificProxy, "127.0.0.1", 1080);
            HttpProxy.Startup(37180, false, false);


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
            Task.Run(() => Debug.WriteLine(obj));
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
