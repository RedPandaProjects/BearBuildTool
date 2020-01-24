using BearBuildTool.Tools;
using BearBuildTool.VisualCode;
using BearBuildTool.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
/// <summary>
/// test 2
/// </summary>
namespace BearBuildTool
{
    class BearBuildTool
    {
        static void Help()
        {
            Console.WriteLine("BearBuildTool [options] ([project][configure][platform])");
            Console.WriteLine("BearBuildTool -createvisualproject [project1] [project2] ...");
            Console.WriteLine("configure:");
            Console.WriteLine("\tdebug");
            Console.WriteLine("\tmixed");
            Console.WriteLine("\trelease");
            Console.WriteLine("platform:");
            Console.WriteLine("\twin32");
            Console.WriteLine("\twin64");
            Console.WriteLine("\tlinux32");
            Console.WriteLine("\tlinux64");
            Console.WriteLine("-clean clean project intermediate");
            Console.WriteLine("-cleanall full clean intermediate");
            Console.WriteLine("-withoutwarning");
            Console.WriteLine("-ansi");


            System.Environment.Exit(0);
        }
        static void Initialize()
        {
            
            Config.Global.LoadConfig();
            if(String.IsNullOrEmpty( Config.Global.BasePath)||!Directory.Exists(Config.Global.BasePath))
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        Config.Global.BasePath = fbd.SelectedPath;
                        Config.Global.SaveConfig();
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
         
            Config.Global.IntermediatePath = Path.Combine(Config.Global.BasePath, Config.Global.IntermediatePath);
            if (!Directory.Exists(Config.Global.IntermediatePath))
            {
                Directory.CreateDirectory(Config.Global.IntermediatePath);
            }
            Config.Global.IntermediatePath = Path.GetFullPath(Config.Global.IntermediatePath);

            Config.Global.BinariesPath = Path.Combine(Config.Global.BasePath, Config.Global.BinariesPath);

            if (!Directory.Exists(Config.Global.BinariesPath))
            {
                Directory.CreateDirectory(Config.Global.BinariesPath);
            }
            Config.Global.BinariesPath = Path.GetFullPath(Config.Global.BinariesPath);

            Config.Global.ProjectsPath = Path.GetFullPath(Path.Combine(Config.Global.BasePath, Config.Global.ProjectsPath));

            Config.Global.ProjectsCSFile = new Dictionary<string, string>();

            Projects.ProjectsReader.Read();
            Projects.ExecutableReader.Read();
        }
        public static void CleanProject()
        {
            if (Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure][Config.Global.DevVersion].ContainsKey(Config.Global.Project) == false)
            {
                throw new Exception(String.Format("Приложение {0} не существует!!", Config.Global.Project));
            }

            Console.WriteLine(String.Format("Начало чистки проекта[{0}] конфигурация[{1}] платформа[{2}]", Config.Global.Project, Config.Global.Configure.ToString(), Config.Global.Platform.ToString()));
            Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediatePath, Config.Global.Platform.ToString());
            if (Directory.Exists(Config.Global.IntermediateProjectPath))
            {
                Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediateProjectPath, Config.Global.Configure.ToString());
                if (Directory.Exists(Config.Global.IntermediateProjectPath))
                {
                    Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediateProjectPath, Config.Global.Project);
                    if (Directory.Exists(Config.Global.IntermediateProjectPath))
                    {
                        Directory.Delete(Config.Global.IntermediateProjectPath, true);
                        Console.WriteLine(String.Format("Чистка завершина."));
                        return;
                    }
                }
              
            }
            Console.WriteLine(String.Format("Чистка невозможно ,потому-что проект не найден!"));
        }
        public static void CompileProject()
        {
            if (Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure][Config.Global.DevVersion].ContainsKey(Config.Global.Project) == false)
            {
                throw new Exception(String.Format("Приложение {0} не существует!!", Config.Global.Project));
            }

            Console.WriteLine(String.Format("Начало сборки проекта[{0}] конфигурация[{1}] платформа[{2}]", Config.Global.Project, Config.Global.Configure.ToString(), Config.Global.Platform.ToString()));
            var time = DateTime.Now.TimeOfDay;
            Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediatePath, Config.Global.Platform.ToString());
            if (!Directory.Exists(Config.Global.IntermediateProjectPath))
            {
                Directory.CreateDirectory(Config.Global.IntermediateProjectPath);
            }
            Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediateProjectPath, Config.Global.Configure.ToString());
            if (!Directory.Exists(Config.Global.IntermediateProjectPath))
            {
                Directory.CreateDirectory(Config.Global.IntermediateProjectPath);
            }
            Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediateProjectPath, Config.Global.Project);
            if (!Directory.Exists(Config.Global.IntermediateProjectPath))
            {
                Directory.CreateDirectory(Config.Global.IntermediateProjectPath);
            }

            Config.Global.BinariesPlatformPath = Path.Combine(Config.Global.BinariesPath, Config.Global.Platform.ToString());
            if (!Directory.Exists(Config.Global.BinariesPlatformPath))
            {
                Directory.CreateDirectory(Config.Global.BinariesPlatformPath);
            }
            Config.Global.OutProjectFileName = Config.Global.Project;
            if (Config.Global.Configure != Config.Configure.Release)
            {
                Config.Global.OutProjectFileName += "_" + Config.Global.Configure.ToString();
            }
            if (Config.Global.Platform == Config.Platform.Linux)
            {
                Config.Global.BuildTools = new Linux.LinuxBuildTools();
            }
            else
            {
                if(Config.Global.Platform == Config.Platform.MinGW)
                {
                    Config.Global.BuildTools = new Windows.MinGWBuildTool();
                }
                else if (Config.Global.Platform == Config.Platform.Clang)
                {
                    Config.Global.BuildTools = new Windows.ClangBuildTool();
                }
                else
                {
                    Config.Global.BuildTools = new Windows.VCBuildTools();
                    if (Config.Global.Windows10SDKUsing)
                    {
                        Console.WriteLine("Windows 10 SDK:{0}", VCBuildTools.GetWindows10SDKVersion(VCBuildTools.FindWindowsSDKInstallationFolder()));
                    }

                }
              
            }

           
            //    SourceFile file = new SourceFile();
            /*      List<string> Linclude = new List<string>();
                  Linclude.AddRange( File.ReadAllLines(@"E:\list.txt"));
                  DateTime dateTime = DateTime.MinValue;
                  file.CheakSource(ref Linclude, @"E:\GameDev\BearSDK\intermediate\Win32\Debug\stalker\stalkercop_script", @"E:\GameDev\BearSDK\projects\engine\stalker\stalkercop\source_script\pch_script.cpp", ref dateTime);
               */
            Projects.Build build = new Projects.Build();
            build.ProjectBuild(Config.Global.Project);
            build.AutonomousProjectsBuild();
            if (Config.Global.CountBuild == 0)
            {
                Console.WriteLine("Не требует сборки");
            }
            else
            {
                time = DateTime.Now.TimeOfDay-time;
                Console.WriteLine(String.Format("Сборка завершена: количество {0} Время потрачено {1}", Config.Global.CountBuild,time.ToString()));
            }
        }

        internal static void GenerateProjectFileVC(string name)
        {
            Console.WriteLine("Создание проектов трансляторов для VisaulCode");
            VCProjectGenerate projectFile = new VCProjectGenerate();
          
            projectFile.Generate(name);
        }

        public static void GenerateProjectFileVS(string name)
        {
            Console.WriteLine("Создание проектов трансляторов для VisaulStudio 2017");
            VSProjectGenerate projectFile = new VSProjectGenerate();
            projectFile.Generate(name);
        }
        public static void GenerateProjectFileVC(string name,string platform)
        {
            if(platform.ToLower()=="linux")
            {
                Config.Global.Platform = Config.Platform.Linux;
            }
            else
            {
                throw new Exception(String.Format("Не поддерживает {0}", platform));

            }
            Config.Global.BinariesPlatformPath = Path.Combine(Config.Global.BinariesPath, Config.Global.Platform.ToString());
            if (!Directory.Exists(Config.Global.BinariesPlatformPath))
            {
                Directory.CreateDirectory(Config.Global.BinariesPlatformPath);
            }
            Console.WriteLine("Создание проектов трансляторов для VisaulCode");
            VCProjectGenerate projectFile = new VCProjectGenerate();
            projectFile.Generate(name);
        }
        public static void ClaenProject()
        {
            Console.WriteLine(String.Format("Начало очистки проекта[{0}] конфигурация[{1}] платформа[{2}]", Config.Global.Project, Config.Global.Configure.ToString(), Config.Global.Platform.ToString()));

            Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediatePath, Config.Global.Platform.ToString());
            if (Directory.Exists(Config.Global.IntermediateProjectPath))
            {

                Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediateProjectPath, Config.Global.Configure.ToString());
                if (Directory.Exists(Config.Global.IntermediateProjectPath))
                {

                    Config.Global.IntermediateProjectPath = Path.Combine(Config.Global.IntermediateProjectPath, Config.Global.Project);
                    if (Directory.Exists(Config.Global.IntermediateProjectPath))
                    {
                        Directory.Delete(Config.Global.IntermediateProjectPath, true);
                    }
                }
            }
        }
        static void CallAction(string arg,string[] args,int i)
        {
         
            if (arg == "-createfilters")
            {
                Initialize();
                if (args.Length != i + 2) return;
                Windows.VisualProject.VisualProject.CreateFilters(args[i], args[i+1]);
                System.Environment.Exit(0);
            }
            else if (arg == "-createvisualproject")
            {
                Initialize();
                if (args.Length != i + 1) return;
                GenerateProjectFileVS(args[i]);
                System.Environment.Exit(0);
            }
            else
          if (arg == "-createvcproject")
            {
                Initialize();
                if (args.Length != i + 2) return;
                GenerateProjectFileVC(args[i], args[i + 1]);
                System.Environment.Exit(0);
            }
            else
          if (arg == "-cleanall")
            {
                Console.WriteLine("Полная чистка");
                Config.Global.IntermediatePath = Path.Combine(Config.Global.BasePath, Config.Global.IntermediatePath);

                if (Directory.Exists(Config.Global.IntermediatePath))
                {
                    Config.Global.IntermediatePath = Path.GetFullPath(Config.Global.IntermediatePath);
                    Directory.Delete(Config.Global.IntermediatePath);
                }
                if (FileSystem.ExistsFile(Path.Combine(Config.Global.BasePath, "Bear.sln")))
                {
                    File.Delete(Path.Combine(Config.Global.BasePath, "Bear.sln"));
                }
                System.Environment.Exit(0);
            }
            else
            {
            }

        }
        static bool SetOption(string arg)
        {
            arg = arg.ToLower();
            if (arg == "-withoutwarning")
            {
                Config.Global.WithoutWarning = true;
                return true;
            } 
            if (arg == "-one_thread")
            {
                Config.Global.CountThreads = 1;
                return true;
            }
            if (arg == "-ansi")
            {
                Config.Global.UNICODE = false;
                return true;
            }
            if (arg == "-clean")
            {
                Config.Global.Clean = true;
                return true;
            }
            if (arg == "-rebuild")
            {
                Config.Global.Rebuild = true;
                return true;
            }

            if (arg[0] == '-')
            {
                Help();
                return true;
            }
            return false;
        }
        [STAThread]
        static void Main(string[] args)
        {
            {
                Config.Global.CountThreads = Environment.ProcessorCount;
            }
            int i = 0;
            for (; i < args.Length; i++)
            {
                CallAction(args[i], args, i + 1);
                if (!SetOption(args[i]))
                {
                    break;
                }
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;
           
            if (args.Length == i) { RunMainForm(); Config.Global.SaveConfig(); return; }



        
            Config.Global.Project = args[i + 0];
            if (!Config.Global.SetConfigure(args[i + 1]) || !Config.Global.SetPlatform(args[i + 2]))
            {
                Help();
            }
            Initialize();
            if (Config.Global.Clean)
            {
                CleanProject();
            }
            else
            {
                CompileProject();
            }
        }

        private static void RunMainForm()
        {
            UI.MainForm mainForm = new UI.MainForm();
            Initialize();
            mainForm.ShowDialog();
        }
    }
}
//test