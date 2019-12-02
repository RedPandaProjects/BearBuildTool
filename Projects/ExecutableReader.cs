using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BearBuildTool.Projects
{
    class ExecutableReader
    {

        public static void Read()
        {
            Config.Global.ExecutableMap = new Dictionary<Config.Platform, Dictionary<Config.Configure, Dictionary<string, Executable>>>();
            Config.Global.Platform = Config.Platform.Win32;
            Read1();
            Config.Global.Platform = Config.Platform.Win64;
            Read1();
            Config.Global.Platform = Config.Platform.MinGW;
            Read1();
            Config.Global.Platform = Config.Platform.Linux;
            Read1();


        }
        private static void Read1()
        {
            
            Config.Global.Configure = Config.Configure.Debug;
            Read2();
            Config.Global.Configure = Config.Configure.Mixed;
            Read2();
            Config.Global.Configure = Config.Configure.Release;
            Read2();

        }
        private static void Read2()
        {
            
            string[] list_path_projects = Directory.GetFiles(Config.Global.ProjectsPath, "*.executable.cs", SearchOption.AllDirectories);
            

            string namedll = "executabls";
            namedll += ".dll";

            Assembly asm = Compiler.CompilerAndLoad(list_path_projects, Path.Combine(Config.Global.IntermediatePath, namedll));
            
            foreach (string file in list_path_projects)
            {
                string name = Path.GetFileName(file);
                name = name.Substring(0, name.Length - 14);
                if (Count == 0)
                    Config.Global.ProjectsCSFile.Add(name, file);
                Executable executable = Activator.CreateInstance(asm.GetType(name), Path.GetDirectoryName(file)) as Executable;
                Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure].Add(name, executable);
                Config.Global.ProjectsMap[Config.Global.Platform][Config.Global.Configure].Add(name, executable);
                if (executable.ProjectPath == null) executable.ProjectPath = Path.GetDirectoryName(file);
            }
        }
    }
}
