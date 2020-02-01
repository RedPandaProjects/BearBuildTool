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
     
        public string ResourceFile;
    };
    class GenerateProjectFile
    {
        public GenerateProjectFile()
        {
            MapProjects = new Dictionary<Config.Platform, Dictionary<Config.Configure, Dictionary<string, ProjectFileInfo>>>();
            MapProjectsForList = new Dictionary<string, bool>();
            PathFileInfo = new Dictionary<string, string>();
            ProjectPath = new Dictionary<string, string>();
        }
        public new Dictionary<Config.Platform, Dictionary<Config.Configure, Dictionary<string, ProjectFileInfo>>> MapProjects;
        public Dictionary<string, string> PathFileInfo;
        public Dictionary<string, string> ProjectPath;
        private Dictionary<string, bool> MapProjectsForList;



        private void GetProjects(string name, ref Dictionary<string,bool> projects,Config.Platform platform,Config.Configure configure,  bool begin = true)
        {
            if (begin) MapProjectsForList = new Dictionary<string, bool>();

            if (!MapProjectsForList.ContainsKey(name))
            {
               if(!projects.ContainsKey(name))
                {
                    projects.Add(name, true);
                }
                MapProjectsForList.Add(name, true);

                var project = Config.Global.ProjectsMap[platform][configure][Config.Global.DevVersion][name];

                foreach (string i in project.Projects.Public)
                {
                    GetProjects(i, ref projects, platform, configure, false);
                };
                foreach (string i in project.Projects.Private)
                {
                    GetProjects(i, ref projects, platform, configure,false);
                };
                foreach (string i in project.IncludeAutonomousProjects)
                {
                    GetProjects(i, ref projects, platform, configure, false);
                }
            }

        }


        public void GetProjects(string name, ref List<string> projects, IEnumerable<Config.Platform> platforms=null, bool begin = true)
        {
            if(platforms==null)
            {
                platforms = new Config.Platform[] {Config.Platform.Win32, Config.Platform.Win64, Config.Platform.MinGW, Config.Platform.Clang, Config.Platform.Linux };

            }
            Dictionary<string, bool> pairs = new Dictionary<string, bool>();
            foreach (Config.Platform platform in platforms)
            {
                GetProjects(name, ref pairs, platform, Config.Configure.Debug, true);
                GetProjects(name, ref pairs, platform, Config.Configure.Mixed, true);
                GetProjects(name, ref pairs, platform, Config.Configure.Release, true);
            }
            projects .AddRange(pairs.Keys);



        }
        public void GetSource(string name, ref List<string> vs)
        {

            Dictionary<string, bool> pairs = new Dictionary<string, bool>();
            foreach (var a in MapProjects.Values)
            {
                foreach (var i in a.Values)
                {
                    foreach (string file in i[name].SourceFile)
                    {
                        if (!pairs.ContainsKey(file))
                        {
                            pairs[file] = true;
                        }
                    }
                }
            }
            vs.AddRange(pairs.Keys);
        }
        public void GetResourceFile(string name, ref List<string> vs)
        {

            Dictionary<string, bool> pairs = new Dictionary<string, bool>();
            foreach (var a in MapProjects.Values)
            {
                foreach (var i in a.Values)
                {
                    if (!String.IsNullOrEmpty( i[name].ResourceFile))
                    if (!pairs.ContainsKey(i[name].ResourceFile))
                    {
                        pairs[i[name].ResourceFile] = true;
                    }
                }
            }
            vs.AddRange(pairs.Keys);
        }
        public void GetInclude(string name, ref List<string> vs)
        {

            Dictionary<string, bool> pairs = new Dictionary<string, bool>();
            foreach (var a in MapProjects.Values)
            {
                foreach (var i in a.Values)
                {
                    foreach (string file in i[name].Include.ToList())
                    {
                        if (!pairs.ContainsKey(file))
                        {
                            pairs[file] = true;
                        }
                    }
                }
            }
            vs.AddRange(pairs.Keys);
        }
        public void GetIncludeFile(string name, ref List<string> vs)
        {

            Dictionary<string, bool> pairs = new Dictionary<string, bool>();
            foreach (var a in MapProjects.Values)
            {
                foreach (var i in a.Values)
                {
                    foreach (string file in i[name].IncludeFile.Keys)
                    {
                        if (!pairs.ContainsKey(file))
                        {
                            pairs[file] = true;
                        }
                    }
                }
            }
            vs.AddRange(pairs.Keys);
        }

        public void RegisterProject(string name, IEnumerable<Config.Platform> platforms = null, bool source_code = true)
        {
            if (platforms == null)
            {
                platforms = new Config.Platform[] { Config.Platform.Win32, Config.Platform.Win64, Config.Platform.MinGW, Config.Platform.Clang, Config.Platform.Linux };

            }
            foreach (Config.Platform platform in platforms)
            {
                RegisterProject(name,  platform, Config.Configure.Debug, source_code);
                RegisterProject(name,  platform, Config.Configure.Mixed, source_code);
                RegisterProject(name,  platform, Config.Configure.Release, source_code);
            }
        }
        private void RegisterProject(string name, Config.Platform platform, Config.Configure configure, bool source_code=true)
        {
            if(!MapProjects.ContainsKey(platform))
            {
                MapProjects[platform] = new Dictionary<Config.Configure, Dictionary<string, ProjectFileInfo>>();

            }

            if(!MapProjects[platform].ContainsKey(configure))
            {
                MapProjects[platform][configure] = new Dictionary<string, ProjectFileInfo>();
            }
            if (!MapProjects[platform][configure].ContainsKey(name))
            {
           
                ProjectFileInfo info = new ProjectFileInfo();
                PathFileInfo[name] = Config.Global.ProjectsCSFile[name];
                List<string> LIncludeFile = new List<string>();
                var project = Config.Global.ProjectsMap[platform][configure][Config.Global.DevVersion][name];
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
                    RegisterProject(i,platform,configure,false);
                    info.Include.Append(MapProjects[platform][configure][i].Include);
                    info.Defines.Append(MapProjects[platform][configure][i].Defines);
                };
                foreach (string i in project.Projects.Private)
                {
                    RegisterProject(i, platform, configure, false);
                    info.Include.AppendInPrivate(MapProjects[platform][configure][i].Include);
                    info.Defines.AppendInPrivate(MapProjects[platform][configure][i].Defines);
                };


                foreach (string i in project.IncludeInProject.Public)
                {
                    info.Include.Append(Config.Global.ProjectsMap[platform][configure][Config.Global.DevVersion][i].Include);
                    
                };
                foreach (string i in project.IncludeInProject.Private)
                {
                    info.Include.AppendInPrivate(Config.Global.ProjectsMap[platform][configure][Config.Global.DevVersion][i].Include);
                };

                Tools.BuildTools.SetGlobalDefines(info.Defines.Private, name.ToUpper(), PathFileInfo[name].IndexOf(".project.cs") != -1 ? BuildType.StaticLibrary : BuildType.Executable);
                if (source_code)
                {
                    info.SourceFile.AddRange(project.Sources);
                }
                if(!ProjectPath.ContainsKey(name))
                ProjectPath[name] = project.ProjectPath;
                MapProjects[platform][configure].Add(name, info);
            }
        }
    }
}
