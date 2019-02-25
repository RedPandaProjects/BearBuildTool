using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BearBuildTool.Projects;

namespace BearBuildTool.Linux
{
    class LinuxBuildTools : Tools.BuildTools
    {
        private string GCCPath = null;
        private string ArPath = null;
        private string RanlibPath = null;
        private string Which(string name)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "/bin/sh";
            proc.StartInfo.Arguments = String.Format("-c 'which {0}'", name);
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;

            proc.Start();
            proc.WaitForExit();

            string path = proc.StandardOutput.ReadLine();

            if (proc.ExitCode == 0 && String.IsNullOrEmpty(proc.StandardError.ReadToEnd()))
            {
                return path;
            }
            return null;
        }
        public LinuxBuildTools()
        {
            GCCPath = Which("g++");
            if (GCCPath == null)
            {
                throw new Exception("Неудалось найти g++");
            }
            ArPath = Which("ar");
            if (ArPath == null)
            {
                throw new Exception("Неудалось найти ar");
            }
            RanlibPath = Which("ranlib");
            if (ArPath == null)
            {
                throw new Exception("Неудалось найти ranlib");
            }

            return;
        }
        public override void BuildObject(List<string> LInclude, List<string> LDefines, string pch, string pchH, bool createPCH, string source, string obj, BuildType buildType)
        {
            
            string Arguments = "";
            /////////////////////////
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
            if (buildType == BuildType.DynamicLibrary)
            {
                Arguments += "-fPIC ";
            }
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
            if (createPCH)
            {
                Arguments += "-x c++-header ";
                Arguments += "-std=c++14 ";
                Arguments += "-o \"" + pch + "\" ";
                BuildObject(LInclude, LDefines, null, null, false, source, obj, buildType);
            }
            else if (Path.GetExtension(source).ToLower() == ".cpp")
            {
                Arguments += "-x c++ ";
                Arguments += "-std=c++14 ";
                Arguments += "-o \"" + obj + "\" ";
                if (pch != null)
                {
                    Arguments += "-include  \"" + Path.Combine(Path.GetDirectoryName(pch), Path.GetFileNameWithoutExtension(pch)) + "\" ";
                }
                if (!Config.Global.WithoutWarning)
                {
                    Arguments += "-Wno-invalid-offsetof ";
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
                Arguments += String.Format("-I\"{0}\" ", include);
            }
            Arguments += String.Format("\"{0}\"",source);
            //
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = GCCPath;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Path.GetFullPath(".");
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
        }
        public override void BuildDynamicLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outDynamicLib, string outStaticLib)
        {
            string Arguments = " ";
            Arguments += string.Format("-o \"{0}\" ", Path.Combine(Path.GetDirectoryName(outDynamicLib), "lib" + Path.GetFileName(outDynamicLib) + ".so"));

            if (Config.Global.Configure != Config.Configure.Release)
            {
                Arguments += "-rdynamic ";
            }
            else
            {
                Arguments += "-s ";
            }

            Arguments += "-shared ";

            List<string> objlist = new List<string>();
            // Arguments += string.Format("-Wl,-rpath-link=\"{0}\"\" ", Path.GetDirectoryName(Executable));
            foreach (string obj in objs)
            {
                objlist.Add(string.Format("\"{0}\"", obj));
            }
            foreach (string path in libsPath)
            {
                // Arguments += string.Format("-Wl,-rpath=\"{0}\" ", path);
                Arguments += string.Format("-L\"{0}\" ", path);
            }
            foreach (string lib in libs)
            {
                Arguments += string.Format(" -l\"{0}\" ", lib);

            }
            File.WriteAllLines(outStaticLib + ".txt", objlist);
            Arguments += string.Format(" -Wl,@\"{0}\"", outStaticLib + ".txt");





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

            if (Config.Global.Configure!=Config.Configure.Release)
            {
                Arguments += "-rdynamic ";   
            }
            else
            {
                Arguments += "-s "; 
            }

            Arguments += "-Wl,--unresolved-symbols=ignore-in-shared-libs ";

            List<string> objlist = new List<string>();
           // Arguments += string.Format("-Wl,-rpath-link=\"{0}\"\" ", Path.GetDirectoryName(Executable));
            foreach (string obj in objs)
            {
                objlist.Add(string.Format("\"{0}\"", obj));
            }
             foreach(string path in libsPath)
            {
               // Arguments += string.Format("-Wl,-rpath=\"{0}\" ", path);
                Arguments += string.Format("-L\"{0}\" ", path);
            }
            foreach (string lib in libs)
            {
                Arguments += string.Format(" -l\"{0}\" ", lib);
            }
            File.WriteAllLines(outStaticLib + ".txt", objlist);
            Arguments += string.Format(" -Wl,@\"{0}\"", outStaticLib + ".txt");

       

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
                files.Add(String.Format("\"{0}\"", obj));
            }
            File.WriteAllLines(outStaticLib + ".txt", files);
            {

                string Arguments = " rc ";
                Arguments += String.Format("-o \"{0}\" ", Path.Combine(Path.GetDirectoryName(outStaticLib), "lib" + Path.GetFileName(outStaticLib) + ".a"));
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
                Arguments += String.Format("\"{0}\" ", Path.Combine(Path.GetDirectoryName(outStaticLib), "lib" + Path.GetFileName(outStaticLib) + ".a"));
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
        public override void SetDefines(List<string> LDefines, string OutFile, BuildType buildType)
        {
            base.SetDefines(LDefines, OutFile, buildType);
            switch (buildType)
            {
                case BuildType.ConsoleExecutable:
                    break;
                case BuildType.Executable:
                    break;
                case BuildType.StaticLibrary:
                    break;
                case BuildType.DynamicLibrary:
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
                case Config.Platform.Linux:
                    LDefines.Add("LINUX");
                    LDefines.Add("_LINUX64");
                    LDefines.Add("X64");
                    break;
            }

        }
    }

}
