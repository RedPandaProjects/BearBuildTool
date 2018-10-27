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
            string[] list_path_projects = Directory.GetFiles(Config.Global.ProjectsPath, "*.project.cs", SearchOption.AllDirectories);
            
            Config.Global.ProjectsMap = new Dictionary<string, Project>();
            if (list_path_projects == null || list_path_projects.Length == 0) return;
            Assembly asm = Compiler.CompilerAndLoad(list_path_projects, Path.Combine(Config.Global.IntermediatePath, "projects.dll"));
            foreach (string file in list_path_projects)
            {
                string name = Path.GetFileName(file);
                name = name.Substring(0, name.Length - 11);
                Config.Global.ProjectsCSFile.Add(name, file);
                Config.Global.ProjectsMap.Add(name, (Project)Activator.CreateInstance(asm.GetType(name), Path.GetDirectoryName(file)));
            }
        }
    }
}
