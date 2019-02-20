using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BearBuildTool.Tools
{
    class SourceFile
    {

        public static bool CheakSource(List<string> LInclude, string LIntermediate, string file, ref DateTime dateTime)
        {
            string listFile = Path.Combine(LIntermediate, Path.GetFileName(file) + ".txt");
            return CheaklcudeInfo(LInclude,file, listFile,ref  dateTime);
        }
        private static bool CheaklcudeInfo(List<string> LInclude, string source, string listFileName,ref DateTime dateTime)
        {
            ListFiles listFile = new ListFiles(listFileName);
            DateTime dateTime1 = DateTime.MinValue;
            DateTime dateTime2 = FileSystem.GetLastWriteTime(listFileName);
            DateTime dateTime3 = FileSystem.GetLastWriteTime(source);
            bool ReCreate = !listFile.GetFilesMaxDate(ref dateTime1);
            if (!FileSystem.ExistsFile(listFileName) || ReCreate || dateTime1 > dateTime2 || dateTime3>dateTime2)
            {
                 CreateInlcudeInfo(LInclude,source, listFileName);
                dateTime = dateTime3;
                return true;
            }
            dateTime= dateTime3;
            return false;
        }
        private static void CreateInlcudeInfo(List<string> LInclude, string source,string listFileName)
        {
            ListFiles listFile = new ListFiles(listFileName);
            includeMap = new Dictionary<string, bool>();
            listFile.ClearFiles();
            WriteIncludeInfo(LInclude, ref listFile, source);
            listFile.Write();
        }
        private static Dictionary<string, bool> includeMap;
        private static void WriteIncludeInfo(List<string> LInclude, ref ListFiles listFile, string infile)
        {
            string text = File.ReadAllText(infile);
            int index = 0;
            for (string inc = ReadIncludeInText(text, ref index); index != -1; inc = ReadIncludeInText(text, ref index))
            {
                if (inc != null)
                {
                    bool find = false;
                    string filename = inc;
                    if (filename.IndexOf('\n') !=-1) continue;
                    if (filename.IndexOf('\r') != -1) continue;
                    foreach (string path in LInclude)
                    {
                        if (FileSystem.ExistsFile(Path.Combine(path  , filename)))
                        {
                            string file =Path.GetFullPath( Path.Combine(path, filename));
                            if (!includeMap.ContainsKey(file))
                            {
                                includeMap.Add(file, true);
                                listFile.Files.Add(file);
                                WriteIncludeInfo(LInclude, ref listFile, Path.Combine(path, filename));
                             
                            }
                            
                          
                            find = true;
                            break;
                        }
                    }
                    {
                        string path = Path.GetDirectoryName(infile);
                        if (!find && FileSystem.ExistsFile(Path.Combine(path, filename)))
                        {
                            string file = Path.GetFullPath(Path.Combine(path, filename));
                            if (!includeMap.ContainsKey(file))
                            {
                                includeMap.Add(file, true);
                                listFile.Files.Add(file);
                                WriteIncludeInfo(LInclude, ref listFile, Path.Combine(path, filename));
                            }
                           
                        }
                    }
                }
            }
        }
        static string ReadIncludeInText(string text, ref int index)
        {
            FindIncludeInText(text, ref index);
            if (index >= 0)
            {
                int indexEnd = 0;
                if (text.Length > index)
                    while (Char.IsWhiteSpace(text[index]))
                        index++;
                if (text.Length <= index + 1) return null;
                if (text[index] == '"') indexEnd = text.IndexOf('"', index + 1);
                else if (text[index] == '<') indexEnd = text.IndexOf('>', index + 1);
                else return null;
                if (indexEnd > index) { return text.Substring(index + 1, indexEnd - index - 1); }
            }
            return null;
        }



        static void FindIncludeInText(string text, ref int index)
        {
            while (index >= 0)
            {
                index = text.IndexOf('#', index);
                if (index < 0)
                {
                    index = -1;
                    return;
                }
                index++;
                if (text.Length > index)
                    while (Char.IsWhiteSpace(text[index]))
                        index++;
                if (text.Length > index + 7 && text.Substring(index, 7) == "include")
                {
                    index = index + 7;
                    return;
                }

            }
            index = -1;
        }
    }
}
