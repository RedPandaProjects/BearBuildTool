using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BearBuildTool.Tools
{
    class FileSystem
    {
        static Mutex MutexDate = new Mutex();
        static Mutex MutexExist = new Mutex();
        static Dictionary<string, DateTime> mapDateTime=null;
        static Dictionary<string, bool> mapExist = null;
        public static  void Clear()
        {
            mapDateTime = null;
            mapExist = null;
        }
        public static DateTime GetLastWriteTime(string file)
        {
            MutexDate.WaitOne();
            if (mapDateTime == null)
            {
                mapDateTime = new Dictionary<string, DateTime>();
            }
            if (mapDateTime.ContainsKey(file))
            {
                var r =  mapDateTime[file];
                MutexDate.ReleaseMutex();
                return r;

            }
            else
            {

                if (ExistsFile(file))
                {
                    var date = File.GetLastWriteTime(file);

                    mapDateTime.Add(file, date);
                    MutexDate.ReleaseMutex();
                    return date;
                }
                MutexDate.ReleaseMutex();
                return DateTime.MinValue;
            }
        }
        public static bool ExistsFile(string file)
        {
            MutexExist.WaitOne();
            if (mapExist == null)
            {
                mapExist = new Dictionary<string, bool>();
            }
            if (mapExist.ContainsKey(file))
            {
                var r=  mapExist[file];
                MutexExist.ReleaseMutex();
                return r;

            }
            else
            {
                bool b= File.Exists(file);
                mapExist.Add(file, b);
                MutexExist.ReleaseMutex();
                return b;
            }
        }
    }
}
