using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BearBuildTool.Tools
{
    class FileSystem
    {
        static Dictionary<string, DateTime> mapDateTime=null;
        static Dictionary<string, bool> mapExist = null;
        public static DateTime GetLastWriteTime(string file)
        {
            if (mapDateTime == null)
            {
                mapDateTime = new Dictionary<string, DateTime>();
            }
            if (mapDateTime.ContainsKey(file))
            {
                return mapDateTime[file];

            }
            else
            {

                if (ExistsFile(file))
                {
                    var date = File.GetLastWriteTime(file);

                    mapDateTime.Add(file, date);
                    return date;
                }
                return DateTime.MinValue;
            }
        }
        public static bool ExistsFile(string file)
        {
            if (mapExist == null)
            {
                mapExist = new Dictionary<string, bool>();
            }
            if (mapExist.ContainsKey(file))
            {
                return mapExist[file];

            }
            else
            {
                bool b= File.Exists(file);
                if(b)
                {
                    mapExist.Add(file, b);
                }
                return b;
            }
        }
    }
}
