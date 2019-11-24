using BearBuildTool.Projects;
using BearBuildTool.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BearBuildTool.Windows
{
    class VCBuildTools : Tools.BuildTools
    {
        string VCToolPath;
        string CCompiler;
        string Linker;
        string LibraryLinker;
        string ConsoleOut;
        string ResourceBuilder;
        public override BuildTools Create()
        {
            return new VCBuildTools();
        }
        static bool FindKey(string key, string val, out string path)
        {
            string str = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\" + key, val, null) as string;
            if (!String.IsNullOrEmpty(str))
            {
                path = str;
                return true;
            }
            str = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\" + key, val, null) as string;
            if (!String.IsNullOrEmpty(str))
            {
                path = str;
                return true;
            }
            str = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Wow6432Node\\" + key, val, null) as string;
            if (!String.IsNullOrEmpty(str))
            {
                path = str;
                return true;
            }
            str = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\" + key, val, null) as string;
            if (!String.IsNullOrEmpty(str))
            {
                path = str;
                return true;
            }
            path = null;
            return false;
        }
        private string GetVS2017Path()
        {
            string path;
            if (FindKey("Microsoft\\VisualStudio\\SxS\\VS7", "15.0", out path) == false)
            {
                throw new Exception("Visual Studio 2017 неустановлена.");
            }
            return path;
        }
        private string GetVC2017Path()
        {
            string path;
            if (FindKey("Microsoft\\VisualStudio\\SxS\\VC7", "15.0", out path) == true)
            {
                if (FileSystem.ExistsFile(Path.Combine(path, "Auxiliary", "Build", "Microsoft.VCToolsVersion.default.txt")) == false)
                {
                    throw new Exception("С++ 14.1  неустановлен.");
                }
                string version1 = File.ReadAllText(Path.Combine(path, "Auxiliary", "Build", "Microsoft.VCToolsVersion.default.txt")).Trim();
                return Path.Combine(path, "Tools", "MSVC", version1);

            }
            path = GetVS2017Path();
            path += "VC\\";
            if (FileSystem.ExistsFile(Path.Combine(path, "Auxiliary", "Build", "Microsoft.VCToolsVersion.default.txt")) == false)
            {
                throw new Exception("С++ 14.1  неустановлен.");
            }
            string version = File.ReadAllText(Path.Combine(path, "Auxiliary", "Build", "Microsoft.VCToolsVersion.default.txt")).Trim();
            return Path.Combine(path, "Tools", "MSVC", version);

        }
        private void FindUniversalCRT(out string UniversalCRTDir, out string UniversalCRTVersion)
        {

            string[] RootKeys =
            {
                "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots",
                "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots",
                "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows Kits\\Installed Roots",
                "HKEY_CURRENT_USER\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows Kits\\Installed Roots",
            };

            List<DirectoryInfo> IncludeDirs = new List<DirectoryInfo>();
            foreach (string RootKey in RootKeys)
            {
                string IncludeDirString = Registry.GetValue(RootKey, "KitsRoot10", null) as string;
                if (IncludeDirString != null)
                {
                    DirectoryInfo IncludeDir = new DirectoryInfo(Path.Combine(IncludeDirString, "include"));
                    if (IncludeDir.Exists)
                    {
                        IncludeDirs.AddRange(IncludeDir.EnumerateDirectories());
                    }
                }
            }

            DirectoryInfo LatestIncludeDir = IncludeDirs.OrderBy(x => x.Name).LastOrDefault(n => n.Name.All(s => (s >= '0' && s <= '9') || s == '.') && Directory.Exists(n.FullName + "\\ucrt"));
            if (LatestIncludeDir == null)
            {
                UniversalCRTDir = null;
                UniversalCRTVersion = null;
            }
            else
            {
                UniversalCRTDir = LatestIncludeDir.Parent.Parent.FullName;
                UniversalCRTVersion = LatestIncludeDir.Name;
            }
        }
        private static string FindNetFxSDKExtensionInstallationFolder()
        {
            string[] Versions;
            Versions = new string[] { "4.6", "4.6.1", "4.6.2" };

            foreach (string Version in Versions)
            {
                string Result = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SDKs\NETFXSDK\" + Version, "KitsInstallationFolder", null) as string
                            ?? Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\NETFXSDK\" + Version, "KitsInstallationFolder", null) as string;
                if (Result != null)
                {
                    return Result.TrimEnd('\\');
                }
            }

            return string.Empty;
        }
        public static string FindWindowsSDKInstallationFolder()
        {
            string Version = "v8.1";
            if (Config.Global.Windows10SDKUsing) Version = "v10.0";
             var Result =
                    Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Microsoft SDKs\Windows\" + Version, "InstallationFolder", null)
                ?? Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\" + Version, "InstallationFolder", null)
                ?? Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\" + Version, "InstallationFolder", null);

            if (Result == null)
            {
                throw new Exception(String.Format("Windows SDK {0} неустановлен.", Version));
            }

            return (string)Result;
        }
        public static string FindWindows10SDKInstallationFolder()
        {
            string  Version = "v10.0";
            var Result =
                   Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Microsoft SDKs\Windows\" + Version, "InstallationFolder", null)
               ?? Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\" + Version, "InstallationFolder", null)
               ?? Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\" + Version, "InstallationFolder", null);

            if (Result == null)
            {
                throw new Exception(String.Format("Windows SDK {0} неустановлен.", Version));
            }

            return (string)Result;
        }
        private static string GetWindowsSDKInstallationLibFolder(string WindowsSDKDir)
        {
            if (Config.Global.Windows10SDKUsing)
            {
                return Windows10SDKVersion;
            }
            return "winv6.3";
        }

        private static string FindWindowsSDKExtensionFolder(string WindowsSDKDir)
        {
            if (Config.Global.Windows10SDKUsing)
            {
                string Version = "v10.0";
                // Based on VCVarsQueryRegistry
                string FinalResult = null;
                {
                    object Result = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows SDKs\" + Version, "InstallationFolder", null)
                              ?? Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows SDKs\" + Version, "InstallationFolder", null);
                    if (Result == null)
                    {
                        Result = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\" + Version, "InstallationFolder", null)
                              ?? Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Wow6432Node\Microsoft\Microsoft SDKs\Windows\" + Version, "InstallationFolder", null);
                    }
                    if (Result != null)
                    {
                        FinalResult = ((string)Result).TrimEnd('\\');
                    }

                }
                if (FinalResult == null)
                {
                    FinalResult = string.Empty;
                }
            }

            return WindowsSDKDir;
        }
        public static string Windows10SDKVersion = null;
        private static string GetWindowsSDKIncludePath(string WindowsSDKDir,string IncludePath)
        {
            if (Config.Global.Windows10SDKUsing)
            {
                DirectoryInfo IncludeDir = new DirectoryInfo(Path.Combine(WindowsSDKDir, "include", Windows10SDKVersion, IncludePath));
                if (!IncludeDir.Exists)
                {
                    throw new Exception(String.Format("Папка [{0}] для Windows 10 SDK  не найдена", IncludeDir.FullName));
                }


                return Path.Combine(WindowsSDKDir, "include", Windows10SDKVersion, IncludePath);
            }
            return Path.Combine(WindowsSDKDir, "include", IncludePath);
        }
        public static IEnumerable<string> GetWindows10SDKs(string WindowsSDKDir)
        {
            DirectoryInfo IncludeDir = new DirectoryInfo(Path.Combine(WindowsSDKDir, "include"));
            if (!IncludeDir.Exists)
            {
                throw new Exception(String.Format("Папка [{0}] для Windows 10 SDK  не найдена", IncludeDir.FullName));
            }

            var LatestIncludeDir = IncludeDir.EnumerateDirectories();
            List<Version> Versions = new List<Version>();
            foreach (var Dir in LatestIncludeDir)
            {
                Version version;
                if (Version.TryParse(Dir.Name, out version))
                {
                    if (Directory.Exists(Path.Combine(WindowsSDKDir, "include", Dir.Name, "ucrt")) &&
                        Directory.Exists(Path.Combine(WindowsSDKDir, "include", Dir.Name, "um")) &&
                        Directory.Exists(Path.Combine(WindowsSDKDir, "include", Dir.Name, "winrt")) &&
                        Directory.Exists(Path.Combine(WindowsSDKDir, "include", Dir.Name, "shared")))
                        Versions.Add(version);
                }

            }
            Versions.Sort();

            if (Versions.Count == 0)
            {
                throw new Exception(String.Format("Windows 10 SDK  не найден", IncludeDir.FullName));
            }

            List<string> SVersions = new List<string>();
            foreach (var ver in Versions)
            {
                SVersions.Add(ver.ToString());
            }
            return SVersions;
        }
        public static string GetWindows10SDKVersion(string WindowsSDKDir)
        {
            if (Config.Global.Windows10SDKUsing)
            {
                DirectoryInfo IncludeDir = new DirectoryInfo(Path.Combine(WindowsSDKDir, "include"));
                if (!IncludeDir.Exists)
                {
                    throw new Exception(String.Format("Папка [{0}] для Windows 10 SDK  не найдена", IncludeDir.FullName));
                }
            
                var LatestIncludeDir = IncludeDir.EnumerateDirectories();
                List<Version> Versions = new List<Version>();
                foreach (var Dir in LatestIncludeDir)
                {
                    Version version;
                    if (Version.TryParse(Dir.Name, out version))
                    {
                        if (Directory.Exists(Path.Combine(WindowsSDKDir, "include", Dir.Name, "ucrt")) &&
                            Directory.Exists(Path.Combine(WindowsSDKDir, "include", Dir.Name, "um")) &&
                            Directory.Exists(Path.Combine(WindowsSDKDir, "include", Dir.Name, "winrt")) &&
                            Directory.Exists(Path.Combine(WindowsSDKDir, "include", Dir.Name, "shared")))
                        {
                            Versions.Add(version);
                            if (version.ToString() == Config.Global.Windows10SDK)
                            {
                                Windows10SDKVersion = version.ToString();
                                return Windows10SDKVersion;
                            }
                        }
                    }
                }
                Versions.Sort();

                if (Versions.Count==0)
                {
                    throw new Exception(String.Format("Папка [{0}] для Windows 10 SDK  не найдена", IncludeDir.FullName));
                }

                   Windows10SDKVersion  = Versions.Last().ToString();
                return Windows10SDKVersion;
            }
            return String.Empty; ;
        }
        public VCBuildTools()
        {
            string VCPath = GetVC2017Path();
            string UniversalCRTDir;
            string VerisonCRTDir;
            string WindowsSDKDir;
            string WindowsSDKDirExtension;
            string VCPlatformPath = null;
            VCToolPath = null;
            {

                if (Config.Global.Platform == Config.Platform.Win64)
                {
                    string testPath = Path.Combine(VCPath, "bin", "HostX64", "x64", "cl.exe");
                    if (FileSystem.ExistsFile(testPath))
                    {
                        VCToolPath = testPath;

                    }
                    else
                    {
                        testPath = Path.Combine(VCPath, "bin", "HostX86", "x64", "cl.exe");
                        if (FileSystem.ExistsFile(testPath))
                        {
                            VCToolPath = testPath;
                            VCPlatformPath = Path.Combine(VCPath, "bin", "HostX86", "x86");
                        }
                        else throw new Exception("Нет x64 битного компилятора");
                    }


                }
                else
                {


                    string testPath = Path.Combine(VCPath, "bin", "HostX64", "x86", "cl.exe");
                    if (FileSystem.ExistsFile(testPath))
                    {
                        VCToolPath = testPath;
                        VCPlatformPath = Path.Combine(VCPath, "bin", "HostX64", "x64");
                    }
                    else
                    {
                        testPath = Path.Combine(VCPath, "bin", "HostX86", "x86", "cl.exe");
                        if (FileSystem.ExistsFile(testPath))
                        {
                            VCToolPath = testPath;
                        }
                        else throw new Exception("Нет x86 битного компилятора");
                    }


                }
            }
            VCToolPath = Path.GetDirectoryName(VCToolPath);
            CCompiler = Path.Combine(VCToolPath, "cl.exe");
            Linker = Path.Combine(VCToolPath, "link.exe");
            LibraryLinker = Path.Combine(VCToolPath, "lib.exe");

            WindowsSDKDir = FindWindowsSDKInstallationFolder();
            WindowsSDKDirExtension = FindWindowsSDKExtensionFolder(WindowsSDKDir);
            FindUniversalCRT(out UniversalCRTDir, out VerisonCRTDir);
            if (Config.Global.Windows10SDKUsing) GetWindows10SDKVersion(WindowsSDKDir);

            string Paths = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!Paths.Split(';').Any(x => String.Compare(x, VCToolPath, true) == 0))
            {
   
                if (VCPlatformPath != null) Paths = VCToolPath + ";" + VCPlatformPath+";"+ Paths;
                else Paths = VCToolPath + ";" + Paths;
                Environment.SetEnvironmentVariable("PATH", Paths);
            }

            List<string> includes = new List<string>();
            string path = Path.Combine(VCPath, "include");
            if (Directory.Exists(path))
            {
                includes.Add(path);
            }
            path = Path.Combine(VCPath, "atlmfc", "include");
            if (Directory.Exists(path))
            {
                includes.Add(path);
            }
            path = Path.Combine(VCPath, "atlmfc", "include");
            if (Directory.Exists(path))
            {
                includes.Add(path);
            }
            if (!String.IsNullOrEmpty(UniversalCRTDir) && !String.IsNullOrEmpty(VerisonCRTDir))
            {
                includes.Add(Path.Combine(UniversalCRTDir, "include", VerisonCRTDir, "ucrt"));
            }
            string NetFxSDKExtensionDir = FindNetFxSDKExtensionInstallationFolder();
            if (!String.IsNullOrEmpty(NetFxSDKExtensionDir))
            {
                includes.Add(Path.Combine(NetFxSDKExtensionDir, "include", "um")); // 2015
            }
            includes.Add(GetWindowsSDKIncludePath(WindowsSDKDir, "shared")); // 2015
            includes.Add(GetWindowsSDKIncludePath(WindowsSDKDir, "um")); // 2015
            includes.Add(GetWindowsSDKIncludePath(WindowsSDKDir, "winrt")); // 2015
            string ExistingIncludePaths = Environment.GetEnvironmentVariable("INCLUDE");
            if (ExistingIncludePaths != null)
            {
                includes.AddRange(ExistingIncludePaths.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            }

            Environment.SetEnvironmentVariable("INCLUDE", String.Join(";", includes));

            List<string> LibraryPaths = new List<string>();

            if (Config.Global.Platform == Config.Platform.Win32)
            {

                string StdLibraryDir = Path.Combine(VCPath, "lib", "x86");
                if (Directory.Exists(StdLibraryDir))
                {
                    LibraryPaths.Add(StdLibraryDir);
                }
                StdLibraryDir = Path.Combine(VCPath, "atlmfc", "lib", "x86");
                if (Directory.Exists(StdLibraryDir))
                {
                    LibraryPaths.Add(StdLibraryDir);
                }
            }
            else
            {
                string StdLibraryDir = Path.Combine(VCPath, "lib", "x64");
                if (Directory.Exists(StdLibraryDir))
                {
                    LibraryPaths.Add(StdLibraryDir);
                }
                StdLibraryDir = Path.Combine(VCPath, "atlmfc", "lib", "x64");
                if (Directory.Exists(StdLibraryDir))
                {
                    LibraryPaths.Add(StdLibraryDir);
                }
            }

            if (!string.IsNullOrEmpty(NetFxSDKExtensionDir))
            {
                if (Config.Global.Platform == Config.Platform.Win32)
                {
                    LibraryPaths.Add(Path.Combine(NetFxSDKExtensionDir, "lib", "um", "x86"));
                }
                else
                {
                    LibraryPaths.Add(Path.Combine(NetFxSDKExtensionDir, "lib", "um", "x64"));
                }
            }
            if (Config.Global.Platform == Config.Platform.Win32)
            {
                LibraryPaths.Add(Path.Combine(WindowsSDKDir, "lib", GetWindowsSDKInstallationLibFolder(WindowsSDKDir), "um", "x86"));
            }
            else
            {
                LibraryPaths.Add(Path.Combine(WindowsSDKDir, "lib", GetWindowsSDKInstallationLibFolder(WindowsSDKDir), "um", "x64"));

            }
            if (!String.IsNullOrEmpty(UniversalCRTDir) && !String.IsNullOrEmpty(VerisonCRTDir))
            {
                if (Config.Global.Platform == Config.Platform.Win32)
                {
                    LibraryPaths.Add(Path.Combine(UniversalCRTDir, "lib", VerisonCRTDir, "ucrt", "x86"));
                }
                else
                {
                    LibraryPaths.Add(Path.Combine(UniversalCRTDir, "lib", VerisonCRTDir, "ucrt", "x64"));
                }
            }
            // Add the existing library paths
            string ExistingLibraryPaths = Environment.GetEnvironmentVariable("LIB");
            if (ExistingLibraryPaths != null)
            {
                LibraryPaths.AddRange(ExistingLibraryPaths.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            }
            Environment.SetEnvironmentVariable("LIB", String.Join(";", LibraryPaths));


            if (Config.Global.Platform == Config.Platform.Win64)
            {
                ResourceBuilder =  Path.Combine(WindowsSDKDirExtension, "bin","x64","rc.exe");
            }
          
            else
            {
                ResourceBuilder = Path.Combine(WindowsSDKDirExtension, "bin", "x86", "rc.exe");
            }

        }
        public override void BuildResource(List<string> LInclude, List<string> LDefines, string source, string obj, BuildType buildType)
        {

            string cmdLine = "";
            if (Config.Global.Platform == Config.Platform.Win64)
            {
                cmdLine += " /D _WIN64";
            }
            
            cmdLine += " /l 0x409";
            
            foreach (string include in LInclude)
            {
                cmdLine += String.Format(" /i \"{0}\"", include);
            }

            foreach (string define in LDefines)
            {
                cmdLine += String.Format(" /d \"{0}\"", define);
            }
            cmdLine += String.Format(" /fo \"{0}\"", obj);
            cmdLine += String.Format(" \"{0}\"", source);


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = ResourceBuilder;
            process.StartInfo.Arguments = cmdLine;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = Path.GetFullPath(".");
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            ConsoleOut = "";
            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += Process_OutputDataReceived;
            while (process.HasExited == false) { }
            if (process.ExitCode != 0)
            {
                System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");

                System.Console.WriteLine(ConsoleOut);

                System.Console.WriteLine("-----------------------------------------------------------------");
                throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
            }

        }
        public override void BuildStaticLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outStaticLib)
        {
            string Arguments = "";
            Arguments += "/NOLOGO ";
            if (Config.Global.Configure == Config.Configure.Release)
                Arguments += "/LTCG ";
            if (Config.Global.Platform == Config.Platform.Win64)
                Arguments += "/MACHINE:x64 ";
            else
                Arguments += "/MACHINE:x86 ";
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
                listObject.Add(String.Format("\"{0}\"", lib));
            }
            File.WriteAllLines(outStaticLib + ".txt", listObject);
            Arguments += "@" + outStaticLib + ".txt" + " ";
            //test
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = LibraryLinker;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = VCToolPath;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            ConsoleOut = "";
            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += Process_OutputDataReceived;
            while (process.HasExited == false) { }
            if (process.ExitCode != 0)
            {
                System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");

                System.Console.WriteLine(ConsoleOut);

                System.Console.WriteLine("-----------------------------------------------------------------");
                throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
            }
        }
        public override void BuildObject(string NameProject,List<string> LInclude, List<string> LDefines, string pch, string pchH, bool createPCH, string source, string obj, BuildType buildType)
        {

            string Arguments = "";
            Arguments += "/c ";
            if (pch != null)
            {
                if (createPCH)
                {
                    Arguments += String.Format("/Yc\"{0}\" ", pchH);
                }
                else
                {
                    Arguments += String.Format("/Yu\"{0}\" ", pchH);
                }
                Arguments += String.Format("/Fp\"{0}\" ", pch);
            }
            Arguments += "/GS ";
            if (!Config.Global.WithoutWarning)
            {
                Arguments += "/W4 ";
                Arguments += "/WX ";
            }
            else
            {
                Arguments += "/W0 ";
                Arguments += "/WX- ";
            }
            Arguments += "/Zc:wchar_t  ";
            Arguments += "/Zi  ";
            Arguments += "/Gm- ";
            Arguments += "/Zc:inline  ";
            Arguments += "/fp:fast ";
            Arguments += "/errorReport:prompt ";
            Arguments += "/Zc:forScope ";
            Arguments += "/Gd  ";
            Arguments += "/FC   ";
            Arguments += "/nologo ";
            Arguments += "/diagnostics:classic  ";
            if(pch!=null)
                Arguments += "/sdl- ";
            else
                Arguments += "/sdl- ";

            switch (Config.Global.Configure)
            {
                case Config.Configure.Debug:
                    Arguments += "/JMC ";
                    Arguments += "/Od ";
                    Arguments += "/RTC1 ";
                    Arguments += "/MDd  ";
                    Arguments += "/EHsc ";
                    break;
                case Config.Configure.Mixed:
                    Arguments += "/Gy ";
                    Arguments += "/O2 ";
                    Arguments += "/Oy- ";
                    Arguments += "/MD ";
                    Arguments += "/EHsc ";
                    break;
                case Config.Configure.Release:
                    Arguments += "/GL ";
                    Arguments += "/Gy ";
                    Arguments += "/Ox ";
                    Arguments += "/Ob2 ";
                    Arguments += "/GT ";
                    Arguments += "/Oy ";
                    Arguments += "/Oi ";
                    Arguments += "/MD ";
                    Arguments += "/Ot ";

                    Arguments += "/Gd ";
                    Arguments += "/Oi ";
                    Arguments += "/Oi ";
                    Arguments += "/Ot ";

                    break;
            };
            Arguments += "/MP ";
            Arguments += "/analyze- ";
            Arguments += "/Zc:inline ";
            Arguments += String.Format("\"{0}\" ", source);
            foreach (string define in LDefines)
            {
                Arguments += String.Format("/D \"{0}\" ", define);
            }
            foreach (string include in LInclude)
            {
                Arguments += String.Format("/I\"{0}\" ", include);
            }
            Arguments += String.Format("/Fo\"{0}\" ", obj);

           
            Arguments += String.Format("/Fd\"{0}\" ", Path.Combine(Path.GetDirectoryName(obj), "vc141.pdb"));
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = CCompiler;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = Path.Combine(Path.GetDirectoryName(obj));
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            ConsoleOut = "";
            process.Start();
            process.BeginOutputReadLine();

            process.OutputDataReceived += Process2_OutputDataReceived;
            while (process.HasExited == false) { }
            if (process.ExitCode != 0)
            {
               
                throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
            }
        }

        private void Process2_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
          if(!String.IsNullOrEmpty( e.Data))System.Console.WriteLine( e.Data);
        }

        List<string> Source = new List<string>();
        public override void BuildObjectsEnd()
        {
            if (Source.Count == 0) return;
            string Arguments = "";
            Arguments += "/c ";
            if (BuildObjects_pch != null)
            {
                Arguments += String.Format("/Yu\"{0}\" ", BuildObjects_pchH);
                Arguments += String.Format("/Fp\"{0}\" ", BuildObjects_pch);
            }
            Arguments += "/GS ";
            if (!Config.Global.WithoutWarning)
            {
                Arguments += "/W4 ";
                Arguments += "/WX ";
            }
            else
            {
                Arguments += "/W0 ";
                Arguments += "/WX- ";
            }
            Arguments += "/Zc:wchar_t  ";
            Arguments += "/Zi  ";
            Arguments += "/Gm- ";
            Arguments += "/Zc:inline  ";
            Arguments += "/fp:fast ";
            Arguments += "/errorReport:prompt ";
            Arguments += "/Zc:forScope ";
            Arguments += "/Gd  ";
            Arguments += "/FC   ";
            Arguments += "/nologo ";
            Arguments += "/diagnostics:classic  ";
            Arguments += "/sdl- ";

            //Arguments += "/FS ";

            switch (Config.Global.Configure)
            {
                case Config.Configure.Debug:
                    Arguments += "/JMC ";
                    Arguments += "/Od ";
                    Arguments += "/RTC1 ";
                    Arguments += "/MDd  ";
                    Arguments += "/EHsc ";
       
                    break;
                case Config.Configure.Mixed:
                    Arguments += "/Gy ";
                    Arguments += "/O2 ";
                    Arguments += "/Oy- ";
                    Arguments += "/MD ";
                    Arguments += "/EHsc ";
                    break;
                case Config.Configure.Release:
                    Arguments += "/GL ";
                    Arguments += "/Gy ";
                    Arguments += "/Ox ";
                    Arguments += "/Ob2 ";
                    Arguments += "/GT ";
                    Arguments += "/Oy ";
                    Arguments += "/Oi ";
                    Arguments += "/MD ";
                    Arguments += "/Ot ";

                    Arguments += "/Gd ";
                    Arguments += "/Oi ";
                    Arguments += "/Oi ";
                    Arguments += "/Ot ";

                    break;
            };
            Arguments += "/MP ";
            Arguments += "/analyze- ";
            Arguments += "/Zc:inline ";
          /*  foreach (string source in Source)
            {
                Arguments += String.Format("\"{0}\" ", source);
            }*/
            File.WriteAllLines(Path.Combine(BuildObjects_objs_out ,"compile"+ ".txt"), Source);
            Arguments += "@" + Path.Combine(BuildObjects_objs_out, "compile" + ".txt") + " ";
            foreach (string define in BuildObjects_LDefines)
            {
                Arguments += String.Format("/D \"{0}\" ", define);
            }
            foreach (string include in BuildObjects_LInclude)

            {
                Arguments += String.Format("/I\"{0}\" ", include);
            }
            //  Arguments += String.Format("/Fo\"{0}\" ", obj);

            
            Arguments += String.Format("/Fd\"{0}\" ", Path.Combine(BuildObjects_objs_out, "vc141.pdb"));
            Process process = new Process();
            process.StartInfo.FileName = CCompiler;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = BuildObjects_objs_out;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            ConsoleOut = "";
    
            process.OutputDataReceived += Process2_OutputDataReceived;
            process.Start();
            process.BeginOutputReadLine();
            while (process.HasExited == false) { }
            if (process.ExitCode != 0)
            {
                throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
            }
            base.BuildObjectsEnd();
        }

        public override void BuildObjectPush( string source)
        {
            Source.Add(String.Format("\"{0}\"", source));
           
        }

        public override void BuildObjectsStart(string PN, List<string> LInclude, List<string> LDefines, string pch, string pchH, string objs_out, BuildType buildType)
        {
            base.BuildObjectsStart(PN,LInclude, LDefines, pch, pchH, objs_out, buildType);
            Source = new List<string>();
        }



   

        public override void BuildDynamicLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outDynamicLib, string outStaticLib)
        {
            string Arguments = "";
            Arguments += "/DLL ";
            Arguments += "/MANIFEST:NO ";
            Arguments += "/NOLOGO ";
            Arguments += "/DEBUG ";
            Arguments += "/errorReport:prompt ";
            Arguments += "/NXCOMPAT ";
            if (Config.Global.Platform == Config.Platform.Win64)
            {
                Arguments += "/MACHINE:x64 ";
            }
            else
            {
                Arguments += "/MACHINE:x86 ";
                Arguments += "/LARGEADDRESSAWARE ";
            }
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
            foreach (string lib in libs)
            {
                listObject.Add(String.Format("\"{0}\"", lib));
            }
            File.WriteAllLines(outStaticLib + ".txt", listObject);
            Arguments += "@" + outStaticLib + ".txt" + " ";
            Arguments += String.Format("/IMPLIB:\"{0}\" ", outStaticLib);
            Arguments += String.Format("/OUT:\"{0}\" ", outDynamicLib);
            Arguments += String.Format("/PDB:\"{0}\"", Path.Combine(Path.GetDirectoryName(outDynamicLib), Path.GetFileNameWithoutExtension(outDynamicLib) + ".pdb"));
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = Linker;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = VCToolPath;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            ConsoleOut = "";
            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += Process_OutputDataReceived;
            while (process.HasExited == false) { }
            if (process.ExitCode != 0)
            {
                System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");

                System.Console.WriteLine(ConsoleOut);

                System.Console.WriteLine("-----------------------------------------------------------------");
                throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
            }
        }
        public override void BuildExecutable(List<string> objs, List<string> libs, List<string> libsPath, string Executable, string outStaticLib, bool Console)
        {
            string Arguments = "";
            Arguments += "/MANIFEST:NO ";
            Arguments += "/NOLOGO ";
            if(Config.Global.Configure!=Config.Configure.Release)
            Arguments += "/DEBUG ";
            Arguments += "/errorReport:prompt ";
            if(Config.Global.Platform==Config.Platform.Win64)
                Arguments += "/MACHINE:x64 ";
            else
                Arguments += "/MACHINE:x86 ";
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
            List<string> listObject=new List<string>();

            foreach (string obj in objs)
            {
                listObject.Add(String.Format("\"{0}\"", obj));
            }
            foreach (string lib in libs)
            {
                listObject.Add(String.Format("\"{0}\"", lib));
            }
            File.WriteAllLines(outStaticLib + ".txt", listObject);
            Arguments += "@" + outStaticLib + ".txt" + " ";
             Arguments += String.Format("/IMPLIB:\"{0}\" ", outStaticLib);
            Arguments += String.Format("/OUT:\"{0}\" ", Executable);
            Arguments += String.Format("/PDB:\"{0}\"",Path.Combine(Path.GetDirectoryName( Executable), Path.GetFileNameWithoutExtension(Executable)+".pdb"));
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = Linker;
            process.StartInfo.Arguments = Arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WorkingDirectory = VCToolPath;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            ConsoleOut = "";
            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += Process_OutputDataReceived;
            while (process.HasExited == false) { }
            if (process.ExitCode != 0)
            {
                System.Console.WriteLine("-------------------------ОТЧЁТ ОБ ОШИБКАХ-------------------------");

                System.Console.WriteLine(ConsoleOut);
                
                System.Console.WriteLine("-----------------------------------------------------------------");
                throw new Exception(String.Format("Ошибка компиляции {0}", process.ExitCode));
            }
        }
        public override void SetDefines(List<string> LDefines,string OutFile, BuildType buildType)
        {
            LDefines.Add("MSVC");
            base.SetDefines(LDefines, OutFile, buildType);
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
                    LDefines.Add("_HAS_EXCEPTIONS=0");
                    break;
            }
            switch (Config.Global.Platform)
            {
                case Config.Platform.Win32:
                    LDefines.Add("WINDOWS");
                    LDefines.Add("WIN32");
                    LDefines.Add("X32");
                    break;
                case Config.Platform.Win64:
                    LDefines.Add("WINDOWS");
                    LDefines.Add("X64");
                    break;
            }
       
        }
        public override void SetLibraries(List<string> libs, BuildType buildType)
        {
            libs.Add("kernel32.lib");
            libs.Add("user32.lib");
            libs.Add("gdi32.lib");
            libs.Add("winspool.lib");
            libs.Add("comdlg32.lib");
            libs.Add("advapi32.lib");
            libs.Add("shell32.lib");
            libs.Add("ole32.lib");
            libs.Add("oleaut32.lib");
            libs.Add("uuid.lib");
            libs.Add("odbc32.lib");
            libs.Add("odbccp32.lib");
        }
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                ConsoleOut +=  e.Data +"\r\n";
            }
        }
    }
}
