﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
            NekoProxy.InitNekoxy();
        }


    }
}
