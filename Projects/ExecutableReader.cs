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

            Config.Global.ExecutableMap = new Dictionary<Config.Platform, Dictionary<Config.Configure, Dictionary<bool, Dictionary<string, Executable>>>>();
            if (Config.Global.Platform != Config.Platform.None && Config.Global.Configure != Config.Configure.None)
            {
                Read3(Config.Global.DevVersion);
                return;
            }
            Read1();
        }
        private static void Read1()
        {


            Config.Global.Platform = Config.Platform.Win32;
            Read2();
            Config.Global.Platform = Config.Platform.Win64;
            Read2();
            Config.Global.Platform = Config.Platform.MinGW;
            Read2();
            Config.Global.Platform = Config.Platform.Linux;
            Read2();
            Config.Global.Platform = Config.Platform.Win32;
            Config.Global.Configure = Config.Configure.Debug;

        }
        private static void Read2()
        {

            Config.Global.Configure = Config.Configure.Debug;
            Read3(true);
            Config.Global.Configure = Config.Configure.Mixed;
            Read3(true);
            Config.Global.Configure = Config.Configure.Release;
            Read3(true);
            Config.Global.Configure = Config.Configure.Debug;
            Read3(false);
            Config.Global.Configure = Config.Configure.Mixed;
            Read3(false);
            Config.Global.Configure = Config.Configure.Release;
            Read3(false);

        }
        private static string[] ListFileProjects = null;
        private static void Read3(bool DevVersion)
        {
            if (Config.Global.ExecutableMap == null) Config.Global.ExecutableMap = new Dictionary<Config.Platform, Dictionary<Config.Configure, Dictionary<bool, Dictionary<string, Executable>>>>();
            if (!Config.Global.ExecutableMap.ContainsKey(Config.Global.Platform))
                Config.Global.ExecutableMap[Config.Global.Platform] = new Dictionary<Config.Configure, Dictionary<bool, Dictionary<string, Executable>>>();
            if (!Config.Global.ExecutableMap[Config.Global.Platform].ContainsKey(Config.Global.Configure))
                Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure] = new Dictionary<string, Executable>();
                Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure] = new Dictionary<bool, Dictionary<string, Executable>>();
            if (!Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure].ContainsKey(DevVersion))
                Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure][DevVersion] = new Dictionary<string, Executable>();

            string namedll = "executabls";;
            namedll += ".dll";
            if (ListFileProjects == null) ListFileProjects = Directory.GetFiles(Config.Global.ProjectsPath, "*.executable.cs", SearchOption.AllDirectories);
            if (ListFileProjects.Length == 0) return;
            Assembly asm = Compiler.CompilerAndLoad(ListFileProjects, Path.Combine(Config.Global.IntermediatePath, namedll));
            
            foreach (string file in ListFileProjects)
            {
                string name = Path.GetFileName(file);
                name = name.Substring(0, name.Length - 14);
                if (!Config.Global.ProjectsCSFile.ContainsKey(name))
                    Config.Global.ProjectsCSFile.Add(name, file);

                Executable executable = Activator.CreateInstance(asm.GetType(name), Path.GetDirectoryName(file)) as Executable;
                Config.Global.ExecutableMap[Config.Global.Platform][Config.Global.Configure][DevVersion].Add(name, executable);
                Config.Global.ProjectsMap[Config.Global.Platform][Config.Global.Configure][DevVersion].Add(name, executable);
                if (executable.ProjectPath == null) executable.ProjectPath = Path.GetDirectoryName(file);
            }
        }
    }
}
