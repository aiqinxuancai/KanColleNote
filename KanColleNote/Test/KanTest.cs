using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KanColleNote.Test
{
    class KanTest
    {
        //

        public static void StartTest ()
        {
            PackReload(@"D:\git\KanColleNote\KanColleNote\PackData\pack1-5出击+远征结果");
        }


        public static void PackReload(string path)
        {
            var files = Directory.GetFiles(path);

            foreach(string item in files)
            {
                Match match = Regex.Match(item, @"\\\d+(-.[^\.]*).json");
                var matchs = match.Groups;
                if (matchs.Count == 2)
                {
                    var filePath = matchs[1].Value.Replace("-", "/");
                    Debug.WriteLine(filePath);
                    DataRoute.RecvRoute(filePath, File.ReadAllText(item));
                }
            }

        }
    }
}
