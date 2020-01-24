using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BearBuildTool.Projects;

namespace BearBuildTool.Windows
{
    class ClangBuildTool : Tools.BuildTools
    {
        public override Tools.BuildTools Create()
        {
            return new ClangBuildTool();
        }
        private string ClangPath = null;
        private string LibPath = null;
        private string LinkPath = null;
        private string ClangBinPath = null;
        private string GetAPP(string name)
        {
            string path =  Path.Combine( Config.Global.ClangPath, "bin",name+".exe");
            if (File.Exists(path))
                return path;
            return null;
        }
        public ClangBuildTool()
        {
            ClangBinPath = Path.Combine(Config.Global.ClangPath, "bin");
            ClangPath = GetAPP("clang++");
            if (ClangPath == null)
            {
                throw new Exception("Неудалось найти g++");
            }
            LibPath = GetAPP("llvm-lib");
            if (LibPath == null)
            {
                throw new Exception("Неудалось найти lib");
            }
            LinkPath = GetAPP("lld-link");
            if (LinkPath == null)
            {
                throw new Exception("Неудалось найти link");
            }



            {
                string WindowsSDKDir = VCBuildTools.FindWindowsSDKInstallationFolder();
                string WindowsSDKDirExtension = VCBuildTools.FindWindowsSDKExtensionFolder(WindowsSDKDir);
                if (Config.Global.Windows10SDKUsing) VCBuildTools.GetWindows10SDKVersion(WindowsSDKDir);


            /*   List<string> includes = new List<string>();
                
                includes.Add(VCBuildTools.GetWindowsSDKIncludePath(WindowsSDKDir, "um")); // 2015
                includes.Add(VCBuildTools.GetWindowsSDKIncludePath(WindowsSDKDir, "winrt")); // 2015
                string ExistingIncludePaths = Environment.GetEnvironmentVariable("INCLUDE");
                if (ExistingIncludePaths != null)
                {
                    includes.AddRange(ExistingIncludePaths.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
                Environment.SetEnvironmentVariable("INCLUDE", String.Join(";", includes));*/

                List<string> LibraryPaths = new List<string>();
                
                    LibraryPaths.Add(Path.Combine(WindowsSDKDir, "lib", VCBuildTools.GetWindowsSDKInstallationLibFolder(WindowsSDKDir), "um", "x64"));
                
                // Add the existing library paths
                string ExistingLibraryPaths = Environment.GetEnvironmentVariable("LIB");
                if (ExistingLibraryPaths != null)
                {
                    LibraryPaths.AddRange(ExistingLibraryPaths.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
                {
                    Environment.SetEnvironmentVariable("LIB", String.Join(";", LibraryPaths));
                };

            }

            string Paths = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!Paths.Split(';').Any(x => String.Compare(x, ClangBinPath, true) == 0))
            {

                if (ClangBinPath != null) Paths = ClangBinPath + ";" + Paths;
                else Paths = ClangBinPath + ";" + Paths;
                Environment.SetEnvironmentVariable("PATH", Paths);
            }



            return;
        }

     



        public override async Task BuildObject(string PN,List<string> LInclude, List<string> LDefines, string pch, string pchH, bool createPCH, string source, string obj, BuildType buildType, bool warning)
        {
            string Arguments = " ";
            /////////////////////////

            Arguments += " -c ";
            Arguments += "-pipe ";
            /////////////////////////
            Arguments += "-DPLATFORM_EXCEPTIONS_DISABLED=0 ";

            /////////////////////////
            if (!Config.Global.WithoutWarning && warning)
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
                Arguments += "-w ";
            }
            Arguments += "-funwind-tables ";             
            Arguments += "-Wsequence-point ";
            Arguments += "-mmmx -msse -msse2 ";
            Arguments += "-fno-math-errno ";
            Arguments += "-ffast-math -mfpmath=sse -mavx ";
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

            Arguments += "--target=x86_64-pc-windows-msv  ";
            /* switch (Config.Global.Platform)
             {
                 case Config.Platform.Clang32:
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
                Arguments += "-Wa,-mbig-obj ";
                Arguments += "-x c++ ";
                Arguments += "-std=c++17 ";
                Arguments += "-o \"" + obj + "\" ";
                if (pch != null)
                {
                    Arguments += " -include "  +  Path.GetFileNameWithoutExtension(pch).Replace('\\', '/') +" ";
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
            process.StartInfo.FileName = ClangPath;
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
            if(createPCH) await BuildObject(PN, LInclude, LDefines, pch, pchH, false, source, obj, buildType,warning) ;
        }
        public override void BuildDynamicLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outDynamicLib, string outStaticLib)
        {
            string Arguments = "";
            Arguments += "-flavor link ";

            Arguments += "/DLL ";
            Arguments += "/MANIFEST:NO ";
            Arguments += "/NOLOGO ";
            Arguments += "/DEBUG ";
            Arguments += "/errorReport:prompt ";
            Arguments += "/NXCOMPAT ";
            Arguments += "/MACHINE:x64 ";
            if (Config.Global.Configure == Config.Configure.Release)
            {
                Arguments += "/RELEASE ";
                Arguments += "/OPT:ICF ";
                Arguments += "/OPT:REF ";
                Arguments += "/INCREMENTAL:NO ";
            }
            else
            {
                Arguments += "/OPT:NOREF ";
                Arguments += "/OPT:NOICF ";
                Arguments += "/INCREMENTAL ";
            }


            foreach (string libpath in libsPath)
            {
                Arguments += String.Format("/LIBPATH:\"{0}\" ", libpath);
            }
            List<string> listObject = new List<string>();

            foreach (string obj in objs)
            {
                listObject.Add(String.Format("\"{0}\"", obj));
            }
           // libs.Add("libcmt.lib");
            foreach (string lib in libs)
            {
                if (String.IsNullOrEmpty(Path.GetExtension(lib)))
                    listObject.Add(String.Format("\"{0}.lib\"", lib));
                else
                    listObject.Add(String.Format("\"{0}\"", lib));
            }
            File.WriteAllLines(outStaticLib + ".txt", listObject);
            Arguments += "@" + outStaticLib + ".txt" + " ";
            Arguments += String.Format("/IMPLIB:\"{0}\" ", outStaticLib);
            Arguments += String.Format("/OUT:\"{0}\" ", outDynamicLib);
            Arguments += String.Format("/PDB:\"{0}\"", Path.Combine(Path.GetDirectoryName(outDynamicLib), Path.GetFileNameWithoutExtension(outDynamicLib) + ".pdb"));

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = LinkPath;
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
            string Arguments = "";
            Arguments += "/MANIFEST:NO ";
            Arguments += "/NOLOGO ";
            if (Config.Global.Configure != Config.Configure.Release)
                Arguments += "/DEBUG ";
            Arguments += "/errorReport:prompt ";
                Arguments += "/MACHINE:x64 ";
            if (Console)
                Arguments += "/SUBSYSTEM:CONSOLE ";
            else
                Arguments += "/SUBSYSTEM:WINDOWS ";
            if (Config.Global.Configure == Config.Configure.Debug)
            {
                Arguments += "/OPT:NOREF ";
                Arguments += "/OPT:NOICF ";
                Arguments += "/INCREMENTAL:NO ";
            }
            else
            {
                Arguments += "/OPT:REF ";
                Arguments += "/INCREMENTAL ";
            }

            foreach (string libpath in libsPath)
            {
                Arguments += String.Format("/LIBPATH:\"{0}\" ", libpath);
            }
            List<string> listObject = new List<string>();

            foreach (string obj in objs)
            {
                listObject.Add(String.Format("\"{0}\"", obj));
            }
            foreach (string lib in libs)
            {
                if (String.IsNullOrEmpty(Path.GetExtension(lib)))
                    listObject.Add(String.Format("\"{0}.lib\"", lib));
                else
                    listObject.Add(String.Format("\"{0}\"", lib));
            }
            File.WriteAllLines(outStaticLib + ".txt", listObject);
            Arguments += "@" + outStaticLib + ".txt" + " ";
            Arguments += String.Format("/IMPLIB:\"{0}\" ", outStaticLib);
            Arguments += String.Format("/OUT:\"{0}\" ", Executable);
            Arguments += String.Format("/PDB:\"{0}\"", Path.Combine(Path.GetDirectoryName(Executable), Path.GetFileNameWithoutExtension(Executable) + ".pdb"));
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = LinkPath;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = ClangBinPath;
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

            string Arguments = "";
            Arguments += "/NOLOGO ";
            if (Config.Global.Configure == Config.Configure.Release)
                Arguments += "/LTCG ";
                Arguments += "/MACHINE:x64 ";
            foreach (string libpath in libsPath)
            {
                Arguments += String.Format("/LIBPATH:\"{0}\" ", libpath);
            }
            Arguments += String.Format("/OUT:\"{0}\" ", outStaticLib);

            List<string> listObject = new List<string>();

            foreach (string obj in objs)
            {
                listObject.Add(String.Format("\"{0}\"", obj));
            }
            foreach (string lib in libs)
            {
                if (String.IsNullOrEmpty(Path.GetExtension(lib)))
                    listObject.Add(String.Format("\"{0}.lib\"", lib));
                else
                    listObject.Add(String.Format("\"{0}\"", lib));
            }
            File.WriteAllLines(outStaticLib + ".txt", listObject);
            Arguments += "@" + outStaticLib + ".txt" + " ";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = LibPath;
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
        public override void SetLibraries(List<string> libs, BuildType buildType)
        {
            if(buildType==BuildType.Executable|| buildType == BuildType.ConsoleExecutable || buildType == BuildType.DynamicLibrary)
            {
                /*l0ibs.Add("pthread");
                libs.Add("dl");*/
            }
        }
        public override void SetDefines(List<string> LDefines, string OutFile, BuildType buildType)
        {
            base.SetDefines(LDefines, OutFile, buildType);
            LDefines.Add("GCC");
            LDefines.Add("WIN64");
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
                case Config.Platform.Clang:
                    LDefines.Add("WINDOWS");
                    LDefines.Add("X64");
                    break;
            }
        }
    }

}
