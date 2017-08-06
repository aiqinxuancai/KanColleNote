using KanColleNote.Base;
using KanColleNote.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KanColleNote
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string m_runPath;
        App () 
        {
            m_runPath = Directory.GetCurrentDirectory();
            SpeedConfig.Load($@"{m_runPath}\Config.json");

            NekoProxy.InitNekoxy();
            KanSource.Init();

#if DEBUG
            var api_start2 = File.ReadAllText(@"D:\git\KanColleNote\KanColleNote\PackData\pack1-5出击+远征结果\636372285289622-kcsapi-api_start2.json");
            KanDataCore.SetGameStartData(api_start2);
            var name = KanDataCore.GetFurnitureWithId(45);
            Debug.WriteLine(name);
#endif

        }


    }
}
