using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BearBuildTool.Tools
{
    class ListFiles
    {
        public ListFiles(string name)
        {
            Name = name;
            Read();
        }
        public void Read()
        {
            Files = new List<string>();
            if (FileSystem.ExistsFile(Name))
            {
               
                Files.AddRange(File.ReadAllLines(Name));
                // Files.Sort();
            }

        }
        public void Write()
        {
           // Files.Sort();
            File.Delete(Name);
            File.WriteAllLines(Name, Files);
        }
        public bool TestUpdate()
        {
            if(FileSystem.ExistsFile(Name))
            {
                DateTime time = DateTime.MinValue;
                if(GetFilesMaxDate(ref time))
                {
                    if (time < FileSystem.GetLastWriteTime(Name))
                        return false;
                }
                
            }
            return true;
        }
        public void ClearFiles()
        {
            Files = new List<string>();
        }
        public bool TestUpdate(string[] list)
        {
            if (TestUpdate()) return true;
            List<string> files = new List<string>();
            files.AddRange(list);
          //  files.Sort();
            return !Enumerable.SequenceEqual(files, Files);
        }
        public DateTime GetFilesMaxDate()
        {
            DateTime dateTime = DateTime.MinValue;
            GetFilesMaxDate(ref dateTime);
            return dateTime;
        }
        public bool GetFilesMaxDate(ref DateTime dateTime)
        {
            bool ok = true;
            dateTime = DateTime.MinValue;
            foreach (string file in Files)
            {
                if (!FileSystem.ExistsFile(file))
                {
                    ok = false;
                }
                else
                {
                    DateTime dateTimeTemp = FileSystem.GetLastWriteTime(file);
                    if (dateTimeTemp > dateTime)
                        dateTime = dateTimeTemp;
                }
            }
            return ok;
        }
        public string Name;
        public List<string> Files;

    }
}
