using BearBuildTool.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BearBuildTool.Projects
{
    public enum BuildType
    {
        Executable,
        ConsoleExecutable,
        StaticLibrary,
        DynamicLibrary,

    }
    class ProjectInfo
    {
        public BuildType buildType;
        public ProjectListObject Include;
        public ProjectListObject Defines;
        public ProjectListObject LibrariesPath;
        public ProjectListObject LibrariesStatic;
        public ProjectInfo()
        {
            Include = new ProjectListObject();
            Defines = new ProjectListObject();
            LibrariesPath = new ProjectListObject();
            LibrariesStatic = new ProjectListObject();
        }
        public void Append(ProjectInfo projectInfo, bool lib=true)
        {
            Include.Append(projectInfo.Include);
            Defines.Append(projectInfo.Defines);
            if (lib)
            {
                LibrariesPath.Append(projectInfo.LibrariesPath);
                LibrariesStatic.Append(projectInfo.LibrariesStatic);
            }
        }
        public void AppendInPrivate(ProjectInfo projectInfo)
        {
            Include.AppendInPrivate(projectInfo.Include);
            Defines.AppendInPrivate(projectInfo.Defines);
            LibrariesPath.AppendInPrivate(projectInfo.LibrariesPath);
            LibrariesStatic.AppendInPrivate(projectInfo.LibrariesStatic);
        }
        public string LibraryFile;
    };
 
    class Build
    {
        List<string> IncludeAutonomousProjects;
        private Dictionary<string, ProjectInfo> ProjectsInfo;
        public Build()
        {
            ProjectsInfo = new Dictionary<string, ProjectInfo>();
            IncludeAutonomousProjects = new List<string>();
        }
        public void AutonomousProjectsBuild()
        {
            foreach(string name in IncludeAutonomousProjects)
            {
                if (!ProjectsInfo.ContainsKey(name)) ProjectBuild(Config.Global.ProjectsMap[name], name, BuildType.DynamicLibrary);
            }
        }
        public void ProjectBuild(string name)
        {
            if(!ProjectsInfo.ContainsKey(name)) ProjectBuild(Config.Global.ProjectsMap[name], name, GetBuildType(name));
           
        }
        public void ProjectBuildAsStaticLibrary(string name)
        {
            if (!ProjectsInfo.ContainsKey(name)) ProjectBuild(Config.Global.ProjectsMap[name], name,BuildType.StaticLibrary);

        }
        private void ProjectBuild(Project project, string name, BuildType buildType)
        {
            project.StartBuild();
            foreach (string path in project.LibrariesDynamic)
            {
                string file = Path.GetFileName(path);
                string file_new = Path.Combine(Config.Global.BinariesPlatformPath, file);
                if(!FileSystem.ExistsFile(file_new)|| FileSystem.GetLastWriteTime(file) > FileSystem.GetLastWriteTime(file_new))
                {
                    Console.WriteLine(String.Format("Копирование динамической библиотеки {0}", file));
                    File.Copy(path, file_new, true);
                }

            }
            if (project.OnlyAsStatic)
            {
                if (buildType == BuildType.Executable || buildType == BuildType.ConsoleExecutable)
                    throw new Exception("Невозможно перевсти Executable в StaticLibary");
                buildType = BuildType.StaticLibrary;
            }
            ProjectInfo projectInfo = new ProjectInfo();
            projectInfo.buildType = buildType;
            projectInfo.Defines.AppendAll(project.Defines);
            projectInfo.Include.AppendAll(project.Include);
            projectInfo.LibrariesPath.AppendAll(project.LibrariesPath);
            projectInfo.LibrariesStatic.AppendAll(project.LibrariesStatic);

            foreach (string projectName in project.Projects.Public)
            {
                ProjectBuild(projectName);
                projectInfo.Append(ProjectsInfo[projectName]);
                if (ProjectsInfo[projectName].LibraryFile != null)
                    projectInfo.LibrariesStatic.Public.Add(Path.GetFileName( ProjectsInfo[projectName].LibraryFile));
            }
            
            foreach (string projectName in project.Projects.Private)
            {
                ProjectBuild(projectName);
                projectInfo.AppendInPrivate(ProjectsInfo[projectName]);
                if (ProjectsInfo[projectName].LibraryFile != null)
                    projectInfo.LibrariesStatic.Private.Add(Path.GetFileName(ProjectsInfo[projectName].LibraryFile));
            }
            foreach (string projectName in project.IncludeInProject.Public)
            {
                projectInfo.Include.Append(Config.Global.ProjectsMap[projectName].Include);
            }
            foreach (string projectName in project.IncludeInProject.Private)
            {
                projectInfo.Include.AppendInPrivate(Config.Global.ProjectsMap[projectName].Include);
            }
            if(project.IncludeAutonomousProjects.Count!=0)IncludeAutonomousProjects.AddRange(project.IncludeAutonomousProjects);
            Console.WriteLine(String.Format("Сборка проекта {0}", name));
            List<string> LObj = new List<string>();
            List<string> LInclude = projectInfo.Include.ToList();
            
            List<string> LDefines = projectInfo.Defines.ToList();
            List<string> LLibraries = null;
            if (buildType == BuildType.StaticLibrary)
                LLibraries = new List<string>(projectInfo.LibrariesStatic.Private);
            else
                LLibraries = projectInfo.LibrariesStatic.ToList();
            LDefines.Add(String.Format("{0}_EXPORTS", name.ToUpper()));
            List<string> LLibrariesPath = projectInfo.LibrariesPath.ToList();
            string LIntermediate = Path.Combine(Config.Global.IntermediateProjectPath, name);
            if(!Directory.Exists(LIntermediate))
            {
                Directory.CreateDirectory(LIntermediate);
            }
            string PCH = null;
            string PCHH = null;
            string PCHSource = null;
            bool Rebuild = false;
            bool Build = false;
            Config.Global.BuildTools.SetDefines(LDefines, GetOutFile(name, buildType), buildType);
            DateTime dateTimeLibrary = FileSystem.GetLastWriteTime(Config.Global.ProjectsCSFile[name]);

            if (! String.IsNullOrEmpty( project.ResourceFile))
            {
                string obj = Path.Combine(LIntermediate, Path.GetFileNameWithoutExtension(project.ResourceFile) + Config.Global.ObjectExtension);
                if (Config.Global.Rebuild || !FileSystem.ExistsFile(obj) || FileSystem.GetLastWriteTime(project.ResourceFile) > FileSystem.GetLastWriteTime(obj))
                {
                    Console.WriteLine(String.Format("Сборка RES {0}", Path.GetFileName(project.ResourceFile)));
                    Config.Global.BuildTools.BuildResource(LInclude, LDefines, project.ResourceFile, obj, buildType);
                    Build = true;
                }
                LObj.Add(obj);
            }

            if (project.PCHFile != null&&project.PCHIncludeFile!= null)
            {
                if (!FileSystem.ExistsFile(project.PCHFile))
                {
                    throw new Exception(String.Format("Не найден файл {0}", project.PCHFile));
                }
                PCH = Path.Combine(LIntermediate, name + Config.Global.PCHExtension);
                PCHSource = Path.GetFullPath(project.PCHFile);
                PCHH = project.PCHIncludeFile;

                string obj = Path.Combine(LIntermediate, Path.GetFileNameWithoutExtension(PCHSource) + Config.Global.ObjectExtension);
                DateTime dateTime = DateTime.MinValue;
                bool reCreate = SourceFile.CheakSource(LInclude, LIntermediate, PCHSource, ref dateTime);
                if (Config.Global.Rebuild || !FileSystem.ExistsFile(PCH) || !FileSystem.ExistsFile(obj) || reCreate || dateTime > FileSystem.GetLastWriteTime(obj) || dateTime > FileSystem.GetLastWriteTime(PCH)|| dateTimeLibrary> FileSystem.GetLastWriteTime(PCH))
                {

                    Console.WriteLine(String.Format("Сборка PCH {0}", Path.GetFileName(PCHSource)));
                    Config.Global.BuildTools.BuildObject(LInclude, LDefines, PCH, PCHH, true, PCHSource, obj, buildType);
                    Rebuild = true;

                }
                LObj.Add(obj);
            }
            Config.Global.BuildTools.BuildObjectsStart(LInclude, LDefines,PCH, PCHH, LIntermediate, buildType);
            foreach (string source in project.Sources)
            {
                bool C = source.Substring(source.Length - 2, 2).ToLower() == ".c";
                if (C) continue;
                if (project.PCHFile != null && project.PCHIncludeFile != null && source.ToLower() == project.PCHFile.ToLower())
                    continue;
                string obj = Path.Combine(LIntermediate, Path.GetFileNameWithoutExtension(source) + Config.Global.ObjectExtension);
                {
                    DateTime dateTime = DateTime.MinValue;
                    bool reCreate = SourceFile.CheakSource(LInclude, LIntermediate, source, ref dateTime);
                    if (dateTimeLibrary < dateTime) dateTimeLibrary = dateTime;
                    if (Config.Global.Rebuild || (Rebuild && !C) || !FileSystem.ExistsFile(obj) || reCreate || dateTime > FileSystem.GetLastWriteTime(obj))
                    {
                 

                        Config.Global.BuildTools.BuildObjectPush( source);

                        Build = true;
                    }
                }
                LObj.Add(obj);
            }
            Config.Global.BuildTools.BuildObjectsEnd();
            Config.Global.BuildTools.BuildObjectsStart(LInclude, LDefines, null, null, LIntermediate, buildType);
            foreach (string source in project.Sources)
            {
                bool C = source.Substring(source.Length - 2, 2).ToLower() == ".c";
                if (!C) continue;
                if (project.PCHFile != null && project.PCHIncludeFile != null && source.ToLower() == project.PCHFile.ToLower())
                    continue;
                string obj = Path.Combine(LIntermediate, Path.GetFileNameWithoutExtension(source) + Config.Global.ObjectExtension);
                {
                    DateTime dateTime = DateTime.MinValue;
                    bool reCreate = SourceFile.CheakSource(LInclude, LIntermediate, source, ref dateTime);
                    if (dateTimeLibrary < dateTime) dateTimeLibrary = dateTime;
                    if (Config.Global.Rebuild || (Rebuild && !C) || !FileSystem.ExistsFile(obj) || reCreate || dateTime > FileSystem.GetLastWriteTime(obj))
                    {
                        Config.Global.BuildTools.BuildObjectPush(source);
                        Build = true;
                    }
                }
                LObj.Add(obj);
            }
            Config.Global.BuildTools.BuildObjectsEnd();
            LLibrariesPath.Add(Config.Global.IntermediateProjectPath);
            if(Config.Platform.Linux==Config.Global.Platform)
            {
                LLibrariesPath.Add(Config.Global.BinariesPlatformPath);
            }
#if DEBUG
            Dictionary<string, bool> LibPairs=new Dictionary<string, bool>();
#endif
            foreach(string lib in LLibraries)
            {
#if DEBUG
                if(!LibPairs.ContainsKey(lib))
                {
                    LibPairs.Add(lib, true);
                }
                else
                {
                    Console.WriteLine(String.Format("Уже иммеется {0}", lib));
                    continue;
                }
#endif

                string file = GetLib(lib,ref LLibrariesPath);
                if (file != null)
                {
                    DateTime dateTime = FileSystem.GetLastWriteTime(file);
                    if (dateTime > dateTimeLibrary) dateTimeLibrary = dateTime;
                    continue;
                }
                else
                {
                    if (Config.Platform.Linux == Config.Global.Platform)
                    {
                        throw new Exception(string.Format("Не найдена библиотека {0}", lib));
                    }
                }
            }
            string OutFile = GetOutFile( name, buildType);
            string OutFileTemp = OutFile;
            if (project.Sources.Count != 0)
            {
                projectInfo.LibraryFile = GetOutStaticLibrary(name,buildType);
                if(Config.Platform.Linux==Config.Global.Platform)
                {
                    switch (buildType)
                    {
                        case BuildType.StaticLibrary:
                            OutFileTemp = Path.Combine(Path.GetDirectoryName(OutFile), "lib" + Path.GetFileName(OutFile) + ".a");
                            break;
                        case BuildType.DynamicLibrary:
                            OutFileTemp = Path.Combine(Path.GetDirectoryName(OutFile), "lib" + Path.GetFileName(OutFile) + ".so");
                            break;

                    }
                }
                if (Config.Global.Rebuild || (Build || !FileSystem.ExistsFile(OutFileTemp)) || FileSystem.GetLastWriteTime(OutFileTemp) <dateTimeLibrary )
                {

                    Console.WriteLine(String.Format("Сборка {0}", OutFile));
                    Config.Global.CountBuild++;
                    switch (buildType)
                    {

                        case BuildType.Executable:
                        case BuildType.ConsoleExecutable:
                            Config.Global.BuildTools.SetLibraries(LLibraries, buildType);
                            Config.Global.BuildTools.BuildExecutable(LObj, LLibraries, LLibrariesPath, OutFile, GetOutStaticLibrary(name), buildType == BuildType.ConsoleExecutable);
                           
                            break;
                        case BuildType.StaticLibrary:
                            Config.Global.BuildTools.BuildStaticLibrary(LObj, LLibraries, LLibrariesPath, OutFile);
                            break;
                        case BuildType.DynamicLibrary:
                            Config.Global.BuildTools.SetLibraries(LLibraries, buildType);
                            Config.Global.BuildTools.BuildDynamicLibrary(LObj, LLibraries, LLibrariesPath, OutFile, GetOutStaticLibrary(name));
                            break;
                    }


                }
            }
            ProjectsInfo.Add(name, projectInfo);
        }


       
        BuildType GetBuildType(string name)
        {
            if (Config.Global.ExecutableMap.ContainsKey(name))
                return Config.Global.ExecutableMap[name].Console ? BuildType.ConsoleExecutable : BuildType.Executable;
                return BuildType.DynamicLibrary;
        
        }
        public static string GetLib(string lib, ref List<string> paths)
        {
            if (Config.Platform.Linux == Config.Global.Platform)
            {
                string fullPath = Path.Combine( Path.GetDirectoryName(lib), "lib" + Path.GetFileName(lib) + ".a");
                if (!FileSystem.ExistsFile(fullPath))
                {
                    fullPath = Path.Combine( Path.GetDirectoryName(lib), "lib" + Path.GetFileName(lib) + ".so");
                }
                if (FileSystem.ExistsFile(fullPath))
                {
                    return fullPath;
                }

            }
            else
            {
                if (FileSystem.ExistsFile(lib))
                {
                    return lib;
                }
            }

            foreach (string path in paths)
            {

                if (Config.Platform.Linux == Config.Global.Platform)
                {
                   string fullPath = Path.Combine(path, Path.GetDirectoryName(lib), "lib" + Path.GetFileName(lib) + ".a");
                    if (!FileSystem.ExistsFile(fullPath))
                    {
                        fullPath = Path.Combine(path, Path.GetDirectoryName(lib), "lib" + Path.GetFileName(lib) + ".so");
                    }
                    if (FileSystem.ExistsFile(fullPath))
                    {
                        return fullPath;
                    }

                    continue;
                }
                else
                {
                    string fullPath = Path.Combine(path, lib);
                    if (FileSystem.ExistsFile(fullPath))
                    {
                        return fullPath;
                    }

                }
            }
            return null;
        }
        public static string GetOutFile(string name, BuildType buildType)
        {
            string OutName = "";
            if (name == Config.Global.Project)
                OutName = name;
            else
            {
                string gname = Config.Global.Project;
                if (Config.Global.Project.IndexOf('_')!=-1)
                    gname = Config.Global.Project.Substring(0, Config.Global.Project.IndexOf('_'));
                string temp = name;
                if (gname.Length < name.Length && temp.Substring(0, gname.Length) == gname)
                    temp = temp.Substring(gname.Length);
                OutName = Config.Global.Project;
                if (temp[0] != '_')
                    OutName += "_";
                OutName += temp;
            }
            if (Config.Global.Configure != Config.Configure.Release)
                OutName += "_" + Config.Global.Configure.ToString().ToLower();
            switch (buildType)
            {
                case BuildType.Executable:
                case BuildType.ConsoleExecutable:
                    return Path.Combine(Config.Global.BinariesPlatformPath, OutName + Config.Global.ExecutableExtension);
                case BuildType.StaticLibrary:
                    return GetOutStaticLibrary(name);
                case BuildType.DynamicLibrary:
                    return Path.Combine(Config.Global.BinariesPlatformPath, OutName + Config.Global.DynamicLibraryExtension);
            }
            return null;
        }
        private static string GetOutStaticLibrary( string name)
        {
            return Path.Combine(Config.Global.IntermediateProjectPath, name + Config.Global.StaticLibraryExtension);
        }
        private static string GetOutStaticLibrary(string name, BuildType buildType)
        {
            if (Config.Global.Platform == Config.Platform.Linux)
            {
                string OutName = "";
                if (name == Config.Global.Project)
                    OutName = name;
                else
                {
                    string gname = Config.Global.Project;
                    if (Config.Global.Project.IndexOf('_') != -1)
                        gname = Config.Global.Project.Substring(0, Config.Global.Project.IndexOf('_'));
                    string temp = name;
                    if (gname.Length < name.Length && temp.Substring(0, gname.Length) == gname)
                        temp = temp.Substring(gname.Length);
                    OutName = Config.Global.Project;
                    if (temp[0] != '_')
                        OutName += "_";
                    OutName += temp;
                }
                if (Config.Global.Configure != Config.Configure.Release)
                    OutName += "_" + Config.Global.Configure.ToString().ToLower();
                switch (buildType)
                {
                    case BuildType.Executable:
                    case BuildType.ConsoleExecutable:
                        return Path.Combine(Config.Global.BinariesPlatformPath, OutName + Config.Global.ExecutableExtension);
                    case BuildType.StaticLibrary:
                        return GetOutStaticLibrary(name);
                    case BuildType.DynamicLibrary:
                        return Path.Combine(Config.Global.BinariesPlatformPath, OutName + Config.Global.DynamicLibraryExtension);
                }
            }
            else
            {
                return GetOutStaticLibrary(name);
            }
            return null;
        }
    }

}
