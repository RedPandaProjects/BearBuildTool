using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BearBuildTool.Projects
{
    
    class ProjectsReader
    {
        public static void Read()
        {
            Config.Global.ProjectsMap = new Dictionary<Config.Platform, Dictionary<Config.Configure, Dictionary<string, Project>>>();
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
            string[] list_path_projects = Directory.GetFiles(Config.Global.ProjectsPath, "*.project.cs", SearchOption.AllDirectories);
            
            if (list_path_projects == null || list_path_projects.Length == 0) return;

            string namedll = "projects";
            namedll += ".dll";

            Assembly asm = Compiler.CompilerAndLoad(list_path_projects, Path.Combine(Config.Global.IntermediatePath, namedll));
            foreach (string file in list_path_projects)
            {
                string name = Path.GetFileName(file);
                name = name.Substring(0, name.Length - 11);
                    Config.Global.ProjectsCSFile.Add(name, file);
                var projects = (Project)Activator.CreateInstance(asm.GetType(name), Path.GetDirectoryName(file));
                Config.Global.ProjectsMap[Config.Global. .Add(name, projects);
                if (projects.ProjectPath == null) projects.ProjectPath = Path.GetDirectoryName(file);
            }
           
        }
    }
}
