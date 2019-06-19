using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BearBuildTool.Tools
{
    class SourceFile
    {
        //  static Mutex mutex = new Mutex();
        List<string> Includes;
        Dictionary<string, List<string>> TreeInclude = new Dictionary<string, List<string>>();

        public bool CheakSource(ref List<string> LInclude, string LIntermediate, string file, ref DateTime dateTime)
        {
            Includes = LInclude;
            string listFile = Path.Combine(LIntermediate, Path.GetFileName(file) + ".txt");
            return CheaklcudeInfo(file, listFile, ref dateTime);
        }
        private bool CheaklcudeInfo(string source, string listFileName, ref DateTime dateTime)
        {
            //  mutex.WaitOne();
            ListFiles listFile = new ListFiles(listFileName);
            DateTime dateTime1 = DateTime.MinValue;
            DateTime dateTime2 = FileSystem.GetLastWriteTime(listFileName);
            DateTime dateTime3 = FileSystem.GetLastWriteTime(source);
            bool ReCreate = !listFile.GetFilesMaxDate(ref dateTime1);
            if (!FileSystem.ExistsFile(listFileName) || ReCreate || dateTime1 > dateTime2 || dateTime3 > dateTime2)
            {
                CreateInlcudeInfo(source, listFileName);
                dateTime = dateTime3;
                //  mutex.ReleaseMutex();
                return true;
            }
            dateTime = dateTime3;
            //  mutex.ReleaseMutex();
            return false;
        }
        private void CreateInlcudeInfo(string source, string listFileName)
        {
            ListFiles listFile = new ListFiles(listFileName);

            listFile.ClearFiles();

            List<string> list = null;
            Dictionary<string, bool> NoNincludeMap = new Dictionary<string, bool>();
            WriteIncludeInfo(out list, source, ref NoNincludeMap);
            listFile.Files.AddRange(list);
            listFile.Write();
        }
        private void WriteIncludeInfo(out List<string> listFile, string infile, ref Dictionary<string, bool> NoNincludeMap)
        {
            listFile = new List<string>();
            if (NoNincludeMap.ContainsKey(infile)) { return; }
            NoNincludeMap.Add(infile, true);
            Dictionary<string, bool> includeMap = new Dictionary<string, bool>();
            if (TreeInclude.ContainsKey(infile))
            {
                listFile = TreeInclude[infile];
                return;
            }

            {
                bool add = true; string file_path = Path.GetDirectoryName(infile);
                foreach (string path in Includes)
                {
                    if (path == file_path) add = false;
                }
                if (add)
                {
                    Includes.Add(file_path);
                }
            }
            string text = File.ReadAllText(infile);
            int index = 0;
            for (string inc = ReadIncludeInText(text, ref index); index != -1; inc = ReadIncludeInText(text, ref index))
            {
                if (inc != null)
                {
                    bool find = false;
                    string filename = inc;
                    if (filename.IndexOf('\n') != -1) continue;
                    if (filename.IndexOf('\r') != -1) continue;
                    foreach (string path in Includes)
                    {
                        if (FileSystem.ExistsFile(Path.Combine(path, filename)))
                        {
                            string file = Path.GetFullPath(Path.Combine(path, filename));
                            if (!includeMap.ContainsKey(file))
                            {
                                includeMap.Add(file, true);
                                listFile.Add(file);
                                List<string> vs = new List<string>();
                                WriteIncludeInfo(out vs, Path.Combine(path, filename), ref NoNincludeMap);
                                foreach (string v in vs)
                                {
                                    if (!includeMap.ContainsKey(v))
                                    {
                                        includeMap.Add(v, true);
                                        listFile.Add(v);
                                    }
                                }


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
                                listFile.Add(file);
                                List<string> vs = new List<string>();
                                WriteIncludeInfo(out vs, Path.Combine(path, filename), ref NoNincludeMap);
                                foreach (string v in vs)
                                {
                                    if (!includeMap.ContainsKey(v))
                                    {
                                        includeMap.Add(v, true);
                                        listFile.Add(v);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            TreeInclude.Add(infile, listFile);
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
