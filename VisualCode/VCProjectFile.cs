using BearBuildTool.Projects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BearBuildTool.VisualCode
{
    public class VCProjectFile
    {
        bool General = false;
        string Name;
        string ProjectDirectory;
        public string VCDirectory;
        GenerateProjectFile GenerateProject;
        public VCProjectFile(string name, string general_name)
        {
            General = name == general_name;
            Name = name;
            ProjectDirectory = Path.Combine(Config.Global.IntermediatePath, "..", "VisualCode", general_name, name);
            if (!Directory.Exists(ProjectDirectory))
            {
                Directory.CreateDirectory(ProjectDirectory);
            }

            GenerateProject = new GenerateProjectFile();
            GenerateProject.RegisterProject(name);

            VCDirectory = Path.Combine(GenerateProject.MapProjects[Name].ProjectPath, ".vscode");
            if (!Directory.Exists(VCDirectory))
            {
                Directory.CreateDirectory(VCDirectory);
            }
        }

     

        void GenerateTasksMinGW(ref VCTasks vcLaunch, string name, string configure, string platform)
        {
            VCTasks.Task task = new VCTasks.Task();
            task.command= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName).Replace('\\', '/');
            task.args.Add(name);
            task.args.Add(configure);
            task.args.Add(platform);
            task.label = "build " + configure + "_" + platform;
            vcLaunch.tasks.Add(task);
        }

        void GenerateCPPConfigureMinGW(ref VCCppProperties vcCppProperties, string NameConfigurate, string Name, string[] Defines)
        {
            var project = GenerateProject.MapProjects[Name];
            VCCppProperties.Configurate configurate = new VCCppProperties.Configurate();
            List<string> list = project.Include.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].Replace('\\', '/');
            }
            configurate.includePath = list;

            List<string> defines = project.Defines.ToList();
            defines.AddRange(Defines);
            configurate.defines = defines;
            configurate.compilerPath = Path.Combine(Config.Global.MinGWPath, "bin", "g++.exe").Replace('\\', '/');
            configurate.name = NameConfigurate;
            vcCppProperties.configurations.Add(configurate);
        }
        void GenerateLaunchMinGW(ref VCLaunch vcLaunch, string configure, string configure_exe)
        {
            GenerateLaunchMinGW(ref vcLaunch, configure, "MinGW", configure_exe);
        }

        void GenerateLaunchMinGW(ref VCLaunch vcLaunch, string configure, string platform, string configure_exe)
        {
            var project = GenerateProject.MapProjects[Name];
            VCLaunch.Configurate configurate = new VCLaunch.Configurate();
            configurate.name = configure + "_" + platform;

            Config.Global.BinariesPlatformPath = Path.Combine(Config.Global.BinariesPath, platform);
            if (!Directory.Exists(Config.Global.BinariesPlatformPath))
            {
                Directory.CreateDirectory(Config.Global.BinariesPlatformPath);
            }
            if (String.IsNullOrEmpty(configure_exe))
            {
                configurate.program = Path.Combine(Config.Global.BinariesPlatformPath, Name + ".exe").Replace('\\', '/');

            }
            else
            {
                configurate.program = Path.Combine(Config.Global.BinariesPlatformPath, Name + "_" + configure_exe + ".exe").Replace('\\', '/');

            }

            configurate.cwd = Config.Global.BinariesPlatformPath.Replace('\\', '/');
            configurate.miDebuggerPath = Path.Combine(Config.Global.MinGWPath, "bin", "gdb.exe").Replace('\\', '/');
            configurate.preLaunchTask = "build " + configure + "_" + platform; ;
            vcLaunch.configurations.Add(configurate);
        }
        public void Write()
        {

            string[] Platfroms = { "MinGW" };
            string[] Configurations = { "Debug", "Mixed", "Release" };
            if (Config.Global.IsWindows)
            {
                List<string> GlobalDefines = new List<string>();

                GlobalDefines.AddRange(new string[] { "WINDOWS", "LIB", "_LIB" });
                if (Config.Global.UNICODE)
                {
                    GlobalDefines.Add("_UNICODE;");
                    GlobalDefines.Add("UNICODE");
                }


                VCCppProperties vcCppProperties = new VCCppProperties();

                string[][] PlatfromsDefines = {  new string[] { "WIN64", "X64" } };
                string[][] ConfigurationsDefines = { new string[] { "DEBUG", "_DEBUG", "GCC" }, new string[] { "MIXED", "DEBUG", "GCC" }, new string[] { "NDEBUG", "GCC" } };
                {
                    for (int i = 0; i < Platfroms.Length; i++)
                        for (int a = 0; a < Configurations.Length; a++)
                        {

                            List<string> LocalDefines = new List<string>();
                            LocalDefines.AddRange(GlobalDefines);
                            LocalDefines.AddRange(PlatfromsDefines[i]);
                            LocalDefines.AddRange(ConfigurationsDefines[a]);
                            GenerateCPPConfigureMinGW(ref vcCppProperties, Platfroms[i], Name, LocalDefines.ToArray());

                        }
                    File.WriteAllText(Path.Combine(VCDirectory, "c_cpp_properties.json"), JsonConvert.SerializeObject(vcCppProperties, Formatting.Indented));
                }
                if (General)
                {
                    {
                        VCTasks vcTasks = new VCTasks();
                        for (int i = 0; i < Platfroms.Length; i++)
                            for (int a = 0; a < Configurations.Length; a++)
                        {
                                GenerateTasksMinGW(ref vcTasks, Name, Configurations[a], Platfroms[i]);
                        }

                               File.WriteAllText(Path.Combine(VCDirectory, "tasks.json"), JsonConvert.SerializeObject(vcTasks, Formatting.Indented));
                        }
                    {
                        VCLaunch vcLaunch = new VCLaunch();
                        GenerateLaunchMinGW(ref vcLaunch, "Debug", "debug");
                        GenerateLaunchMinGW(ref vcLaunch, "Mixed", "mixed");
                        GenerateLaunchMinGW(ref vcLaunch, "Release", "");
                        File.WriteAllText(Path.Combine(VCDirectory, "launch.json"), JsonConvert.SerializeObject(vcLaunch, Formatting.Indented));

                    }

                }

            }

            else
            {
                {

                }

            }
        }
    }
}
