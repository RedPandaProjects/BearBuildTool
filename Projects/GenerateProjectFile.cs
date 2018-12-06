using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        }
        public ProjectListObject Include;
        public ProjectListObject Defines;
        public List<string> SourceFile;
        public Dictionary<string, bool> IncludeFile;
        public string NameFileInfo;
    };
    class GenerateProjectFile
    {
        public GenerateProjectFile()
        {
            MapProjects = new Dictionary<string, ProjectFileInfo>();
        }
        public Dictionary<string, ProjectFileInfo> MapProjects;
        public void RegisterProject(string name)
        {
            if (!MapProjects.ContainsKey(name))
            {
                ProjectFileInfo info = new ProjectFileInfo();
                info.NameFileInfo = Config.Global.ProjectsCSFile[name];
                List<string> LIncludeFile = new List<string>();
                var project = Config.Global.ProjectsMap[name];
                project.StartBuild();
                foreach (string i in project.Include.Private)
                {
                    string[] files = Directory.GetFiles(i, "*.h", SearchOption.AllDirectories);
                    foreach(string f in files)
                    {
                        if(!info.IncludeFile.ContainsKey(f)) info.IncludeFile.Add(f,true);
                    }
                     files = Directory.GetFiles(i, "*.hpp", SearchOption.AllDirectories);
                    foreach (string f in files)
                    {
                        if (!info.IncludeFile.ContainsKey(f)) info.IncludeFile.Add(f, true);
                    }
                    info.Include.Private.AddRange(project.Include.Private);
                    info.Defines.Private.AddRange(project.Defines.Private);
                }
                foreach (string i in project.Include.Public)
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
                    info.Include.Public.AddRange(project.Include.Public);
                    info.Defines.Public.AddRange(project.Defines.Public);
                }
                foreach (string i in project.Projects.Public)
                {
                    RegisterProject(i);
                    info.Include.Append(MapProjects[i].Include);
                    info.Defines.Append(MapProjects[i].Defines);
                };
                foreach (string i in project.Projects.Private)
                {
                    RegisterProject(i);
                    info.Include.AppendInPrivate(MapProjects[i].Include);
                    info.Defines.AppendInPrivate(MapProjects[i].Defines);
                };
                info.Defines.Private.Add(String.Format("{0}_EXPORTS", name.ToUpper()));
                info.SourceFile.AddRange(project.Sources);
                MapProjects.Add(name, info);
                foreach (string i in project.IncludeAutonomousProjects)
                {
                    RegisterProject(i);
                }
            }
        }
    }
}
