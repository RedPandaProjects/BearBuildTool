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
            if (Config.Global.Platform != Config.Platform.None && Config.Global.Configure != Config.Configure.None)
            {
                Read2();
                return;
            }
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
        private static string[] ListFileProjects = null;
        private static void Read2()
        {
            if (Config.Global.ExecutableMap == null) Config.Global.ProjectsMap = new Dictionary<Config.Platform, Dictionary<Config.Configure, Dictionary<string, Project>>>();
            if (!Config.Global.ExecutableMap.ContainsKey(Config.Global.Platform))
                Config.Global.ExecutableMap[Config.Global.Platform] = new Dictionary<Config.Configure, Dictionary<string, Executable>>();
            if (!Config.Global.ExecutableMap[Config.Global.Platform].ContainsKey(Config.Global.Configure))
                Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure] = new Dictionary<string, Executable>();
            if(ListFileProjects==null) ListFileProjects = Directory.GetFiles(Config.Global.ProjectsPath, "*.executable.cs", SearchOption.AllDirectories);
            

            string namedll = "executabls";
            namedll = namedll + "_" + Config.Global.Platform.ToString() + "_" + Config.Global.Configure.ToString();
            namedll += ".dll";

            Assembly asm = Compiler.CompilerAndLoad(ListFileProjects, Path.Combine(Config.Global.IntermediatePath, namedll));
            
            foreach (string file in ListFileProjects)
            {
                string name = Path.GetFileName(file);
                name = name.Substring(0, name.Length - 14);
                if (!Config.Global.ProjectsCSFile.ContainsKey(name))
                    Config.Global.ProjectsCSFile.Add(name, file);

                Executable executable = Activator.CreateInstance(asm.GetType(name), Path.GetDirectoryName(file)) as Executable;
                Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure].Add(name, executable);
                Config.Global.ProjectsMap[Config.Global.Platform][Config.Global.Configure].Add(name, executable);
                if (executable.ProjectPath == null) executable.ProjectPath = Path.GetDirectoryName(file);
            }
        }
    }
}
