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
            
            string[] list_path_projects = Directory.GetFiles(Config.Global.ProjectsPath, "*.executable.cs", SearchOption.AllDirectories);
            Config.Global.ExecutableMap = new Dictionary<string, Executable>();
            Assembly asm = Compiler.CompilerAndLoad(list_path_projects, Path.Combine(Config.Global.IntermediatePath, "executabls.dll"));
            foreach (string file in list_path_projects)
            {
                string name = Path.GetFileName(file);
                name = name.Substring(0, name.Length - 14);
                Config.Global.ProjectsCSFile.Add(name, file);
                Executable executable = Activator.CreateInstance(asm.GetType(name), Path.GetDirectoryName(file)) as Executable;
                Config.Global.ExecutableMap.Add(name, executable);
                Config.Global.ProjectsMap.Add(name, executable);
                if (executable.ProjectPath == null) executable.ProjectPath = Path.GetDirectoryName(file);
            }
        }
    }
}
