using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BearBuildTool.Projects;

namespace BearBuildTool.Windows
{
    class MinGWBuildTool : Tools.BuildTools
    {
        public override Tools.BuildTools Create()
        {
            return new MinGWBuildTool();
        }
        private string GCCPath = null;
        private string ArPath = null;
        private string RanlibPath = null;
        private string MinGWBinPath = null;
        private string GetAPP(string name)
        {
            string path =  Path.Combine( Config.Global.MinGWPath, "bin",name+".exe");
            if (File.Exists(path))
                return path;
            return null;
        }
        public MinGWBuildTool()
        {
            MinGWBinPath = Path.Combine(Config.Global.MinGWPath, "bin");
            GCCPath = GetAPP("g++");
            if (GCCPath == null)
            {
                throw new Exception("Неудалось найти g++");
            }
            ArPath = GetAPP("ar");
            if (ArPath == null)
            {
                throw new Exception("Неудалось найти ar");
            }
            RanlibPath = GetAPP("ranlib");
            if (ArPath == null)
            {
                throw new Exception("Неудалось найтzzzи ranlib");
            }
            string Paths = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!Paths.Split(';').Any(x => String.Compare(x, MinGWBinPath, true) == 0))
            {

                if (MinGWBinPath != null) Paths = MinGWBinPath + ";"+ Paths;
                else Paths = MinGWBinPath + ";" + Paths;
                Environment.SetEnvironmentVariable("PATH", Paths);
            }
            return;
        }


        List<string> SourceCPP = new List<string>();
        List<string> SourceC = new List<string>();
        public override void BuildObjectsEnd()
        {
            if (SourceCPP.Count != 0)
            {
                string Arguments = " -E ";
                /////////////////////////
                if (BuildObjects_buildType == BuildType.DynamicLibrary || BuildObjects_buildType == BuildType.StaticLibrary)
                {
                    Arguments += "-fPIC ";
                }
                Arguments += " -c ";
                Arguments += "-pipe ";
                /////////////////////////
                Arguments += "-DPLATFORM_EXCEPTIONS_DISABLED=0 ";

                /////////////////////////
                if (!Config.Global.WithoutWarning)
                {
                    Arguments += "-Wall -Werror ";
                    Arguments += "-Wno-sign-compare ";
                    Arguments += "-Wno-enum-compare ";
                    Arguments += "-Wno-return-type ";
                    Arguments += "-Wno-unused-local-typedefs ";
                    Arguments += "-Wno-multichar ";
                    Arguments += "-Wno-unused-but-set-variable ";
                    Arguments += "-Wno-strict-overflow ";
                    Arguments += "-Wno-unused-variable ";
                    Arguments += "-Wno-unused-function ";
                    Arguments += "-Wno-switch ";
                    Arguments += "-Wno-unknown-pragmas ";

                    Arguments += "-Wno-unused-value ";
                }
                else
                {
                    Arguments += "-W ";
                }
                Arguments += "-funwind-tables ";
                Arguments += "-Wsequence-point ";
                Arguments += "-mmmx -msse -msse2 ";
                Arguments += "-fno-math-errno ";

                Arguments += "-fno-strict-aliasing ";

                switch (Config.Global.Configure)
                {
                    case Config.Configure.Debug:
                        Arguments += "-g ";
                        Arguments += "-O0 ";
                        // Arguments += "-gline-tables-only ";
                        Arguments += "-fno-inline ";
                        break;
                    case Config.Configure.Mixed:
                        Arguments += "-g ";
                        Arguments += "-O2 ";
                        //  Arguments += "-gline-tables-only ";
                        break;
                    case Config.Configure.Release:
                        Arguments += "-O2 ";
                        Arguments += "-g0 ";
                        Arguments += "-fomit-frame-pointer ";
                        Arguments += "-fvisibility=hidden ";
                        break;

                }




                {
                    Arguments += "-x c++ ";
                    Arguments += "-std=c++17 ";
                    if (BuildObjects_pch != null)
                    {
                        Arguments += " -include " + Path.GetFileNameWithoutExtension(BuildObjects_pch).Replace('\\', '/');
                    }
                    if (!Config.Global.WithoutWarning)
                    {
                        Arguments += " -Wno-invalid-offsetof ";
                    }
                }

                /* {
                     Arguments += "-x c ";
                 }*/
                foreach (string define in BuildObjects_LDefines)
                {
                    Arguments += String.Format("-D \"{0}\" ", define);
                }
                foreach (string include in BuildObjects_LInclude)
                {
                    Arguments += String.Format("-I\"{0}\" ", include.Replace('\\', '/'));
                }
                List<string> objlist = new List<string>();
                foreach (string obj in SourceCPP)
                {

                    objlist.Add(string.Format("{0}", obj.Replace('\\', '/')));
                }

                File.WriteAllLines(Path.Combine(BuildObjects_objs_out, "compile" + ".txt"), objlist);
                Arguments += string.Format("@\"{0}\"", Path.Combine(BuildObjects_objs_out, "compile" + ".txt").Replace('\\', '/') );


                //
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = GCCPath;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WorkingDirectory = BuildObjects_objs_out;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += Process2_OutputDataReceived;
                process.Start();
                process.BeginOutputReadLine();
                while (process.HasExited == false) { }
                if (process.ExitCode != 0)
                {
                    throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
                }
        
            }
            if (SourceC.Count != 0)
            {
                string Arguments = " -time ";
                /////////////////////////
                if (BuildObjects_buildType == BuildType.DynamicLibrary || BuildObjects_buildType == BuildType.StaticLibrary)
                {
                    Arguments += "-fPIC ";
                }
                Arguments += " -c ";
                Arguments += "-pipe ";
                /////////////////////////
                Arguments += "-DPLATFORM_EXCEPTIONS_DISABLED=0 ";

                /////////////////////////
                if (!Config.Global.WithoutWarning)
                {
                    Arguments += "-Wall -Werror ";
                    Arguments += "-Wno-sign-compare ";
                    Arguments += "-Wno-enum-compare ";
                    Arguments += "-Wno-return-type ";
                    Arguments += "-Wno-unused-local-typedefs ";
                    Arguments += "-Wno-multichar ";
                    Arguments += "-Wno-unused-but-set-variable ";
                    Arguments += "-Wno-strict-overflow ";
                    Arguments += "-Wno-unused-variable ";
                    Arguments += "-Wno-unused-function ";
                    Arguments += "-Wno-switch ";
                    Arguments += "-Wno-unknown-pragmas ";

                    Arguments += "-Wno-unused-value ";
                }
                else
                {
                    Arguments += "-W ";
                }
                Arguments += "-funwind-tables ";
                Arguments += "-Wsequence-point ";
                Arguments += "-mmmx -msse -msse2 ";
                Arguments += "-fno-math-errno ";

                Arguments += "-fno-strict-aliasing ";

                switch (Config.Global.Configure)
                {
                    case Config.Configure.Debug:
                        Arguments += "-g ";
                        Arguments += "-O0 ";
                        // Arguments += "-gline-tables-only ";
                        Arguments += "-fno-inline ";
                        break;
                    case Config.Configure.Mixed:
                        Arguments += "-g ";
                        Arguments += "-O2 ";
                        //  Arguments += "-gline-tables-only ";
                        break;
                    case Config.Configure.Release:
                        Arguments += "-O2 ";
                        Arguments += "-g0 ";
                        Arguments += "-fomit-frame-pointer ";
                        Arguments += "-fvisibility=hidden ";
                        break;

                }

                {
                     Arguments += "-x c ";
                }
                foreach (string define in BuildObjects_LDefines)
                {
                    Arguments += String.Format("-D \"{0}\" ", define);
                }
                foreach (string include in BuildObjects_LInclude)
                {
                    Arguments += String.Format("-I\"{0}\" ", include.Replace('\\', '/'));
                }
                List<string> objlist = new List<string>();
                foreach (string obj in SourceC)
                {

                    objlist.Add(string.Format("{0}", obj.Replace('\\', '/')));
                }

                File.WriteAllLines(Path.Combine(BuildObjects_objs_out, "compile" + ".txt"), objlist);
                Arguments += string.Format("@\"{0}\"", Path.Combine(BuildObjects_objs_out, "compile" + ".txt").Replace('\\', '/') );


                //
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = GCCPath;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WorkingDirectory = BuildObjects_objs_out;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += Process2_OutputDataReceived;
                process.Start();
                process.BeginOutputReadLine();
                while (process.HasExited == false) { }
                if (process.ExitCode != 0)
                {
                    throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
                }

            }
            base.BuildObjectsEnd();
        }

        private void Process2_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data)) System.Console.WriteLine(e.Data);
        }

        public override void BuildObjectPush(string source)
        {
            if(Path.GetExtension(source).ToLower() == ".c")
                SourceC.Add(String.Format("\"{0}\"", source));
            else
                SourceCPP.Add(String.Format("\"{0}\"", source));

        }

        public override void BuildObjectsStart(string PN, List<string> LInclude, List<string> LDefines, string pch, string pchH, string objs_out, BuildType buildType)
        {
            base.BuildObjectsStart(PN, LInclude, LDefines, pch, pchH, objs_out, buildType);
            SourceC = new List<string>();
            SourceCPP = new List<string>();
        }



        public override void BuildObject(string PN,List<string> LInclude, List<string> LDefines, string pch, string pchH, bool createPCH, string source, string obj, BuildType buildType)
        {
           
            string Arguments = " ";
            /////////////////////////
            if (buildType == BuildType.DynamicLibrary||buildType== BuildType.StaticLibrary)
            {
                Arguments += "-fPIC ";
            }
            Arguments += " -c ";
            Arguments += "-pipe ";
            /////////////////////////
            Arguments += "-DPLATFORM_EXCEPTIONS_DISABLED=0 ";

            /////////////////////////
            if (!Config.Global.WithoutWarning)
            {
                Arguments += "-Wall -Werror ";
                Arguments += "-Wno-sign-compare ";
                Arguments += "-Wno-enum-compare ";
                Arguments += "-Wno-return-type ";
                Arguments += "-Wno-unused-local-typedefs ";
                Arguments += "-Wno-multichar ";
                Arguments += "-Wno-unused-but-set-variable ";
                Arguments += "-Wno-strict-overflow ";
                Arguments += "-Wno-unused-variable ";
                Arguments += "-Wno-unused-function ";
                Arguments += "-Wno-switch ";
                Arguments += "-Wno-unknown-pragmas ";
               
                Arguments += "-Wno-unused-value ";
            }
            else
            {
                Arguments += "-W ";
            }
            Arguments += "-funwind-tables ";             
            Arguments += "-Wsequence-point ";
            Arguments += "-mmmx -msse -msse2 ";
            Arguments += "-fno-math-errno ";          
                       
            Arguments += "-fno-strict-aliasing ";
          
            switch (Config.Global.Configure)
            {
                case Config.Configure.Debug:
                    Arguments += "-g ";
                    Arguments += "-O0 ";
                  // Arguments += "-gline-tables-only ";
                    Arguments += "-fno-inline " ;
                    break;
                case Config.Configure.Mixed:
                    Arguments += "-g ";
                    Arguments += "-O2 ";
                  //  Arguments += "-gline-tables-only ";
                    break;
                case Config.Configure.Release:
                    Arguments += "-O2 ";
                    Arguments += "-g0 ";
                    Arguments += "-fomit-frame-pointer ";
                    Arguments += "-fvisibility=hidden ";
                    break;
            
            }


           /* switch (Config.Global.Platform)
            {
                case Config.Platform.MinGW32:
                    Arguments += "-m32 ";
                    break;
            }*/
            if (createPCH)
            {
                Arguments += "-x c++-header ";
                Arguments += "-std=c++17 ";
                Arguments += "-o \"" + pch + "\" ";
             
            }
            else if (Path.GetExtension(source).ToLower() == ".cpp")
            {
                Arguments += "-x c++ ";
                Arguments += "-std=c++17 ";
                Arguments += "-o \"" + obj + "\" ";
                if (pch != null)
                {
                    Arguments += " -include "  +  Path.GetFileNameWithoutExtension(pch).Replace('\\', '/') ;
                }
                if (!Config.Global.WithoutWarning)
                {
                    Arguments += " -Wno-invalid-offsetof ";
                }
            }
            else
            {
                Arguments += "-x c ";
                Arguments += "-o \"" + obj + "\" ";
            }
            foreach (string define in LDefines)
            {
                Arguments += String.Format("-D \"{0}\" ", define);
            }
            foreach (string include in LInclude)
            {
                Arguments += String.Format("-I\"{0}\" ", include.Replace('\\', '/'));
            }

            Arguments += String.Format("\"{0}\"", source.Replace('\\', '/'));
            
            //
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = GCCPath;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(obj);
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string OutConsole = "";
            while(!process.HasExited)
            {
                OutConsole += process.StandardError.ReadToEnd();
                OutConsole += "\n";
            }
            if (process.ExitCode != 0)
            {
                System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");
                System.Console.WriteLine(process.StandardOutput.ReadToEnd());
                System.Console.WriteLine(OutConsole);
                System.Console.WriteLine(process.StandardError.ReadToEnd());
                System.Console.WriteLine("-----------------------------------------------------------------");
                throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
            }
            if(createPCH) BuildObject(PN, LInclude, LDefines, pch, pchH, false, source, obj, buildType);
        }
        public override void BuildDynamicLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outDynamicLib, string outStaticLib)
        {
            string Arguments = " ";
            Arguments += "-shared ";
            Arguments += string.Format("-o \"{0}\" ", Path.Combine(Path.GetDirectoryName(outDynamicLib), "lib" + Path.GetFileName(outDynamicLib) ).Replace('\\','/'));

            if (Config.Global.Configure != Config.Configure.Release)
            {
            }
            else
            {
                Arguments += "-s ";
            }
            
            List<string> objlist = new List<string>();
            foreach (string obj in objs)
            {
                objlist.Add(string.Format("\"{0}\"", obj.Replace('\\', '/')));
            }
            foreach (string path in libsPath)
            {
                Arguments += string.Format("-Wl,-rpath=\"{0}\" ", path.Replace('\\', '/'));
                Arguments += string.Format("-L\"{0}\" ", path.Replace('\\', '/'));
            }
            File.WriteAllLines(outStaticLib + ".txt", objlist);
            Arguments += string.Format(" -Wl,@\"{0}\"", outStaticLib.Replace('\\', '/') + ".txt");
            foreach (string lib in libs)
            {
                Arguments += string.Format(" -l\"{0}\" ",Path.GetFileNameWithoutExtension( lib));

            }
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = GCCPath;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = Path.GetFullPath(".");
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string OutConsole = "";
            while (!process.HasExited)
            {
                OutConsole += process.StandardError.ReadToEnd();
                OutConsole += "\n";
            }
            if (process.ExitCode != 0)
            {
                System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");
                System.Console.WriteLine(process.StandardOutput.ReadToEnd());
                System.Console.WriteLine(OutConsole);
                System.Console.WriteLine(process.StandardError.ReadToEnd());
                System.Console.WriteLine("-----------------------------------------------------------------");
                throw new Exception(String.Format("Ошибка сборки {0}", process.ExitCode));
            }
        }
        public override void BuildExecutable(List<string> objs, List<string> libs, List<string> libsPath, string Executable, string outStaticLib, bool Console)
        {
            string Arguments =  " ";
            Arguments += string.Format("-o \"{0}\" ", Executable);

            if (Config.Global.Configure != Config.Configure.Release)
            {
            }
            else
            {
                Arguments += "-s ";
            }
          

            List<string> objlist = new List<string>();
            foreach (string obj in objs)
            {
                ;
                objlist.Add(string.Format("\"{0}\"", obj.Replace('\\', '/')));
            }
             foreach(string path in libsPath)
            {
                Arguments += string.Format("-Wl,-rpath=\"{0}\" ", path.Replace('\\', '/'));
                Arguments += string.Format("-L\"{0}\" ", path).Replace('\\', '/');
            }
           
            File.WriteAllLines(outStaticLib + ".txt", objlist);
            Arguments += string.Format(" -Wl,@\"{0}\"", outStaticLib.Replace('\\', '/') + ".txt");

            foreach (string lib in libs)
            {
                Arguments += string.Format(" -l\"{0}\" ",Path.GetFileNameWithoutExtension( lib));
            }

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = GCCPath;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = MinGWBinPath;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string OutConsole = "";
            while (!process.HasExited)
            {
                OutConsole += process.StandardError.ReadToEnd();
                OutConsole += "\n";
            }
            if (process.ExitCode != 0)
            {
                System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");
                System.Console.WriteLine(process.StandardOutput.ReadToEnd());
                System.Console.WriteLine(OutConsole);
                System.Console.WriteLine(process.StandardError.ReadToEnd());
                System.Console.WriteLine("-----------------------------------------------------------------");
                throw new Exception(String.Format("Ошибка сборки {0}", process.ExitCode));
            }
        }
        public override void BuildStaticLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outStaticLib)
        {
            List<string> files = new List<string>();
            foreach (string lib in libs)
            {
                string name = Build.GetLib(lib, ref libsPath);
                if (name==null) throw new Exception(String.Format("Не найден файл {0}", Path.GetFileName(lib)));
                files.Add(String.Format("\"{0}\"", name));

            }
            foreach (string obj in objs)
            {
                files.Add(String.Format("\"{0}\"", obj.Replace('\\', '/')));
            }
            File.WriteAllLines(outStaticLib + ".txt", files);
            {

                string Arguments = " rc ";
                Arguments += String.Format("-o \"{0}\" ", Path.Combine(Path.GetDirectoryName(outStaticLib), "lib" + Path.GetFileName(outStaticLib) ).Replace('\\','/'));
                Arguments += String.Format("@\"{0}\"", outStaticLib + ".txt");
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = ArPath;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WorkingDirectory = Path.GetFullPath(".");
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                string OutConsole = "";
                while (!process.HasExited)
                {
                    OutConsole += process.StandardError.ReadToEnd();
                    OutConsole += "\n";
                }
                if (process.ExitCode != 0)
                {
                    System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");
                    System.Console.WriteLine(process.StandardOutput.ReadToEnd());
                    System.Console.WriteLine(OutConsole);
                    System.Console.WriteLine(process.StandardError.ReadToEnd());
                    System.Console.WriteLine("-----------------------------------------------------------------");
                    throw new Exception(String.Format("Ошибка сборки {0}", process.ExitCode));
                }
            }
            {

                string Arguments = " ";
                Arguments += String.Format("\"{0}\" ", Path.Combine(Path.GetDirectoryName(outStaticLib), "lib" + Path.GetFileName(outStaticLib) )).Replace('\\', '/');
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = RanlibPath;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.WorkingDirectory = Path.GetFullPath(".");
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                string OutConsole = "";
                while (!process.HasExited)
                {
                    OutConsole += process.StandardError.ReadToEnd();
                    OutConsole += "\n";
                }
                if (process.ExitCode != 0)
                {
                    System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");
                    System.Console.WriteLine(process.StandardOutput.ReadToEnd());
                    System.Console.WriteLine(OutConsole);
                    System.Console.WriteLine(process.StandardError.ReadToEnd());
                    System.Console.WriteLine("-----------------------------------------------------------------");
                    throw new Exception(String.Format("Ошибка сборки {0}", process.ExitCode));
                }
            }
        }
        public override void SetLibraries(List<string> libs, BuildType buildType)
        {
            if(buildType==BuildType.Executable|| buildType == BuildType.ConsoleExecutable)
            {
                /*l0ibs.Add("pthread");
                libs.Add("dl");*/
            }
        }
        public override void SetDefines(List<string> LDefines, string OutFile, BuildType buildType)
        {
            base.SetDefines(LDefines, OutFile, buildType);
            LDefines.Add("GCC");
            LDefines.Add("MINGW");
            switch (buildType)
            {
                case BuildType.ConsoleExecutable:
                    LDefines.Add("_CONSOLE");
                    break;
                case BuildType.Executable:
                    LDefines.Add("_WINDOWS");
                    break;
                case BuildType.StaticLibrary:
                    LDefines.Add("_LIB");
                    break;
                case BuildType.DynamicLibrary:
                    LDefines.Add("_USRDLL");
                    LDefines.Add("_WINDOWS");
                    break;
            }
            switch (Config.Global.Configure)
            {
                case Config.Configure.Debug:
                    LDefines.Add("_DEBUG");
                    LDefines.Add("DEBUG");
                    break;
                case Config.Configure.Mixed:
                    LDefines.Add("MIXED");
                    LDefines.Add("DEBUG");
                    break;
                case Config.Configure.Release:
                    LDefines.Add("NDEBUG");
                    break;
            }
            switch (Config.Global.Platform)
            {
                case Config.Platform.MinGW:
                    LDefines.Add("WINDOWS");
                    LDefines.Add("X64");
                    break;
            }
        }
    }

}
