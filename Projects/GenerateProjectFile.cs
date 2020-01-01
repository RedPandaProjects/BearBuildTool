using System;
using System.Collections.Generic;
using System.IO;

namespace BearBuildTool.Projects
{
    class ProjectFileInfo
    {
        public ProjectFileInfo()
        {
            Include = new ProjectListObject();
            Defines = new ProjectListObject();
            SourceFile = new List<string>();
            IncludeFile = new Dictionary<string, bool>();
            ResourceFile = String.Empty;
        }
        public ProjectListObject Include;
        public ProjectListObject Defines;
        public List<string> SourceFile;
        public Dictionary<string, bool> IncludeFile;
        public string PathFileInfo;
        public string ResourceFile;
        public string ProjectPath;
    };
    class GenerateProjectFile
    {
        public GenerateProjectFile()
        {
            MapProjects = new Dictionary<string, ProjectFileInfo>();
            MapProjectsForList = new Dictionary<string, bool>();
        }
        public Dictionary<string, ProjectFileInfo> MapProjects;
        private Dictionary<string, bool> MapProjectsForList;

        public void GetProjects(string name, ref List<string> projects, bool begin = true)
        {
            if (begin) MapProjectsForList = new Dictionary<string, bool>();

            if (!MapProjectsForList.ContainsKey(name))
            {
                projects.Add(name);
                MapProjectsForList.Add(name, true);
               
                var project = Config.Global.ProjectsMap[Config.Platform.Win32][Config.Configure.Debug][Config.Global.DevVersion][name];

                foreach (string i in project.Projects.Public)
                {
                    GetProjects(i,ref projects, false);
                };
                foreach (string i in project.Projects.Private)
                {
                    GetProjects(i, ref projects, false);
                };
                foreach (string i in project.IncludeAutonomousProjects)
                {
                    GetProjects(i, ref projects, false);
                }
            }
            
        }
        public void RegisterProject(string name,bool source_code=true)
        {
            if (!MapProjects.ContainsKey(name))
            {
           
                ProjectFileInfo info = new ProjectFileInfo();
                info.PathFileInfo = Config.Global.ProjectsCSFile[name];
            
                List<string> LIncludeFile = new List<string>();
                var project = Config.Global.ProjectsMap[Config.Platform.Win32][Config.Configure.Debug][Config.Global.DevVersion][name];
                project.StartBuild();
                info.ResourceFile = project.ResourceFile;
                foreach (string i in project.Include.Private)
                {
                    if (source_code)
                    {
                        string[] files = Directory.GetFiles(i, "*.h", SearchOption.AllDirectories);
                        foreach (string f in files)
                        {
                            if (!info.IncludeFile.ContainsKey(f)) info.IncludeFile.Add(f, true);
                        }
                        files = Directory.GetFiles(i, "*.hpp", SearchOption.AllDirectories);
                        foreach (string f in files)
                        {
                            if (!info.IncludeFile.ContainsKey(f)) info.IncludeFile.Add(f, true);
                        }
                    }
                
                }
                info.Include.Private.AddRange(project.Include.Private);
                info.Defines.Private.AddRange(project.Defines.Private);
                foreach (string i in project.Include.Public)
                {
                    if (source_code)
                    {
                        string[] files = Directory.GetFiles(i, "*.h", SearchOption.AllDirectories);
                        foreach (string f in files)
                        {
                            if (!info.IncludeFile.ContainsKey(f)) info.IncludeFile.Add(f, true);
                        }
                        files = Directory.GetFiles(i, "*.hpp", SearchOption.AllDirectories);
                        foreach (string f in files)
                        {
                            if (!info.IncludeFile.ContainsKey(f)) info.IncludeFile.Add(f, true);
                        }
                    }
                    
                }
                info.Include.Public.AddRange(project.Include.Public);
                info.Defines.Public.AddRange(project.Defines.Public);
                foreach (string i in project.Projects.Public)
                {
                    RegisterProject(i,false);
                    info.Include.Append(MapProjects[i].Include);
                    info.Defines.Append(MapProjects[i].Defines);
                };
                foreach (string i in project.Projects.Private)
                {
                    RegisterProject(i, false);
                    info.Include.AppendInPrivate(MapProjects[i].Include);
                    info.Defines.AppendInPrivate(MapProjects[i].Defines);
                };


                foreach (string i in project.IncludeInProject.Public)
                {
                    info.Include.Append(Config.Global.ProjectsMap[Config.Platform.Win32][Config.Configure.Debug][Config.Global.DevVersion][i].Include);
                    
                };
                foreach (string i in project.IncludeInProject.Private)
                {
                    info.Include.AppendInPrivate(Config.Global.ProjectsMap[Config.Platform.Win32][Config.Configure.Debug][Config.Global.DevVersion][i].Include);
                };

                info.Defines.Private.Add(String.Format("{0}_EXPORTS", name.ToUpper()));
                if (source_code)
                {
                    info.SourceFile.AddRange(project.Sources);
                }
                info.ProjectPath = project.ProjectPath;
                MapProjects.Add(name, info);
            }
        }
    }
}
