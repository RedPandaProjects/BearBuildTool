using BearBuildTool.Projects;
using System;
using System.Collections.Generic;
using System.IO;

namespace BearBuildTool.Windows.VisualProject
{
    class VisualProject
    {
        GenerateProjectFile GenerateProjectFile = new GenerateProjectFile();
        public Guid Guid;
        string GeneralName;
        string Name;
        string FullPath;
        public string FileVcxproj = String.Empty;
        string FileUser = String.Empty;
        string FileFilters = String.Empty;
        Filters.Project XmlFilers = new Filters.Project();
        User.Project XmlUser = new User.Project();
        Vcxproj.Project XmlVcxproj = new Vcxproj.Project();

        string[] Platfroms = { "Win32", "Win64" };
        string[] PlatfromsDefines = { "WIN32;X32;", "WIN64;X64;" };
        string[] Configurations = { "Debug", "Mixed", "Release" };
        string[] ConfigurationsDefines = { "DEBUG;_DEBUG;", "MIXED;DEBUG;", "NDEBUG;" };
        public VisualProject(string name, string generalName)
        {
            GenerateProjectFile.RegisterProject(name);
            GeneralName = generalName;
            Name = name;
            {
                string LIntermediate = Path.Combine(Config.Global.IntermediatePath, "VCProjects");
                if (!Directory.Exists(LIntermediate))
                {
                    Directory.CreateDirectory(LIntermediate);
                }
                FullPath = Path.Combine(LIntermediate, GeneralName);
                if (!Directory.Exists(FullPath))
                {
                    Directory.CreateDirectory(FullPath);
                }

                FileUser = Path.Combine(FullPath, Name + ".vcxproj.user");
                FileFilters = Path.Combine(FullPath, Name + ".vcxproj.filters");
                FileVcxproj = Path.Combine(FullPath, Name + ".vcxproj");
            }
            Guid = Guid.NewGuid();
        }
        public void Build()
        {
            BuildFilters();
            BuildUser();
            BuildVcxproj();
        }


        void InitVcxproj()
        {
            {
                Vcxproj.ImportGroup importGroup = new Vcxproj.ImportGroup();
                importGroup.Label = Vcxproj.ImportGroup.ELabel.EL_ExtensionSettings;
                XmlVcxproj.importGroups.Add(importGroup);
            }
            {
                Vcxproj.ImportGroup importGroup = new Vcxproj.ImportGroup();
                importGroup.Label = Vcxproj.ImportGroup.ELabel.EL_Shared;
                XmlVcxproj.importGroups.Add(importGroup);
            }
            {
                foreach (string p in Platfroms)
                    foreach (string c in Configurations)
                    {
                        Vcxproj.ImportGroup importGroup = new Vcxproj.ImportGroup();
                        importGroup.Label = Vcxproj.ImportGroup.ELabel.EL_PropertySheets;
                        if (p == "Win64")
                            importGroup.Platform = "x64";
                        else
                            importGroup.Platform = p;
                        importGroup.Configuration = c;
                        XmlVcxproj.importGroups.Add(importGroup);
                    }
            }
            {
                Vcxproj.ImportGroup importGroup = new Vcxproj.ImportGroup();
                importGroup.Label = Vcxproj.ImportGroup.ELabel.EL_ExtensionTargets;
                XmlVcxproj.importGroups.Add(importGroup);
            }
            {
                Vcxproj.PropertyGroup propertyGroup = new Vcxproj.PropertyGroup();
                propertyGroup.Label = Vcxproj.PropertyGroup.ELabel.EL_Globals;
                propertyGroup.Globals.ProjectGuid = Guid;
                propertyGroup.Globals.RootNamespace = GeneralName;
                XmlVcxproj.propertyGroups.Add(propertyGroup);
            }
            {
                foreach (string p in Platfroms)
                    foreach (string c in Configurations)
                    {
                        Vcxproj.PropertyGroup propertyGroup = new Vcxproj.PropertyGroup();
                        propertyGroup.Label = Vcxproj.PropertyGroup.ELabel.EL_Configuration;
                        propertyGroup.Configuration.Configuration = c;
                        if (p == "Win64")
                            propertyGroup.Configuration.Platform = "x64";
                        else
                            propertyGroup.Configuration.Platform = p;

                        XmlVcxproj.propertyGroups.Add(propertyGroup);
                    }
            }
            {
                Vcxproj.ItemGroup itemGroup = new Vcxproj.ItemGroup();
                foreach (string p in Platfroms)
                    foreach (string c in Configurations)
                    {
                        Vcxproj.ProjectConfiguration projectConfiguration = new Vcxproj.ProjectConfiguration();
                        projectConfiguration.Configuration = c;
                        if (p == "Win64")
                            projectConfiguration.Platform = "x64";
                        else
                            projectConfiguration.Platform = p;
                        itemGroup.projectConfigurations.Add(projectConfiguration);
                    }
                XmlVcxproj.itemGroups.Add(itemGroup);

            }
        }

        internal static void SaveFilters(string name, string sub_name)
        {
            string CurrentFilters = String.Empty;
            {
                string LIntermediate = Path.Combine(Config.Global.IntermediatePath, "VCProjects");
                if (!Directory.Exists(LIntermediate))
                {
                    Directory.CreateDirectory(LIntermediate);
                }
                string FullPath = Path.Combine(LIntermediate, name);
                if (!Directory.Exists(FullPath))
                {
                    Directory.CreateDirectory(FullPath);
                }
                CurrentFilters = sub_name + ".vcxproj.filters";
            }
            string ProjectsFilters = String.Empty;
            {
                var projectFileInfo = Config.Global.ProjectsCSFile[sub_name];
                string path = Path.GetDirectoryName(projectFileInfo);
                ProjectsFilters = Path.Combine(path, sub_name + ".vcxproj.filters");
            }
        }

        private void AddFile()
        {
            {
                Vcxproj.ItemGroup itemGroup = new Vcxproj.ItemGroup();
                foreach (string p in GenerateProjectFile.MapProjects[Name].SourceFile)
                {
                    Vcxproj.Files.ClCompile clCompile = new Vcxproj.Files.ClCompile();
                    clCompile.Include = p;
                    itemGroup.clCompiles.Add(clCompile);
                }
                foreach (string i in GenerateProjectFile.MapProjects[Name].IncludeFile.Keys)
                {
                    Vcxproj.Files.ClInclude clInclude = new Vcxproj.Files.ClInclude();
                    clInclude.Include = i;
                    itemGroup.clIncludes.Add(clInclude);
                }
                {
                    Vcxproj.Files.None none = new Vcxproj.Files.None();
                    none.Include = GenerateProjectFile.MapProjects[Name].PathFileInfo;
                    itemGroup.nones.Add(none);
                }

                XmlVcxproj.itemGroups.Add(itemGroup);

            }
        }
        private void AddPropertyGroup()
        {
            var Project = GenerateProjectFile.MapProjects[Name];
            string SInclude = "";
            {
                List<string> LInclude = Project.Include.ToList();
                foreach (var ia in LInclude)
                {
                    SInclude += ia + ";";
                }
            }

            string command = "";
            if (!Config.Global.UNICODE)
            {
                command += "-ansi ";
            }
            if (Config.Global.WithoutWarning)
            {
                command += "-withoutwarning ";
            }

            List<string> LDefines = Project.Defines.ToList();
            for (int i = 0; i < Platfroms.Length; i++)
            {

                string p = Platfroms[i];

                for (int a = 0; a < Configurations.Length; a++)
                {
                    string c = Configurations[a];
                    string defines = "WINDOWS;LIB;_LIB";
                    defines += PlatfromsDefines[i];
                    defines += ConfigurationsDefines[i];
                    if (Config.Global.UNICODE)
                    {
                        defines += "_UNICODE;";
                        defines += "UNICODE;";
                    }
                    foreach (var ia in LDefines)
                    {
                        defines += ia + ";";
                    }
                    Vcxproj.PropertyGroup propertyGroup = new Vcxproj.PropertyGroup();
                    propertyGroup.Label = Vcxproj.PropertyGroup.ELabel.EL_Default;
                    propertyGroup.Default.Configuration = c;
                    if (p == "Win64")
                        propertyGroup.Default.Platform = "x64";
                    else
                        propertyGroup.Default.Platform = p;

                    propertyGroup.Default.OutDir = String.Format("..\\..\\..\\binaries\\{0}\\", p);
                    propertyGroup.Default.IntDir = "..\\..\\..\\binaries\\net\\";
                    propertyGroup.Default.NMakePreprocessorDefinitions = defines;
                    propertyGroup.Default.NMakeIncludeSearchPath = SInclude;
                    propertyGroup.Default.NMakeOutput = String.Format("..\\..\\..\\binaries\\{0}\\{1}_{2}.exe", p, GeneralName, c.ToLower());
                    propertyGroup.Default.NMakeBuildCommandLine = String.Format("..\\..\\..\\binaries\\net\\BearBuildTool.exe {3} {0} {2} {1}", GeneralName, p, c, command);
                    propertyGroup.Default.NMakeReBuildCommandLine = String.Format("..\\..\\..\\binaries\\net\\BearBuildTool.exe {3} -rebuild {0} {2} {1}", GeneralName, p, c, command);
                    propertyGroup.Default.NMakeCleanCommandLine = String.Format("..\\..\\..\\binaries\\net\\BearBuildTool.exe {3} -clean {0} {2} {1}", GeneralName, p, c, command);
                    XmlVcxproj.propertyGroups.Add(propertyGroup);
                }

            }

        }
        void BuildVcxproj()
        {
            InitVcxproj();
            AddFile();
            AddPropertyGroup();
            XmlVcxproj.Save(FileVcxproj);
        }



        void BuildUser()
        {
            User.Project user = new User.Project();
            foreach (string p in Platfroms)
                foreach (string c in Configurations)
                {
                    User.PropertyGroup propertyGroup = new User.PropertyGroup();
                    if (p == "Win64")
                        propertyGroup.Platform = "x64";
                    else
                        propertyGroup.Platform = p;
                    propertyGroup.Configuration = c;
                    propertyGroup.LocalDebuggerWorkingDirectory = String.Format("..\\..\\..\\binaries\\{0}", p);
                    user.propertyGroups.Add(propertyGroup);
                }
            user.Save(FileUser);
        }
      
        void BuildNewFilters()
        {
            Filters.Project filters = new Filters.Project();
            filters.itemGroup.AppendFilter("include");
            filters.itemGroup.AppendFilter("source");
            var i = GenerateProjectFile.MapProjects[Name];
            foreach (var ji in i.SourceFile)
            {
                filters.itemGroup.AppendClCompile(ji, "source");
            }
            foreach (var ji in i.IncludeFile.Keys)
            {
                filters.itemGroup.AppendClInclude(ji, "include");
            }
            filters.Save(FileFilters);
        }
        private void ReBuildFilters(string outfile)
        {
            Filters.Project filters = new Filters.Project();
            filters.Load(outfile);
            var project = GenerateProjectFile.MapProjects[Name];
            {
                for (int i = 0; i < filters.itemGroup.ClCompilers.Count; i++)
                {
                    if (File.Exists(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(FileFilters), filters.itemGroup.ClCompilers[i].Include))))
                        filters.itemGroup.ClCompilers[i].Include = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(FileFilters), filters.itemGroup.ClCompilers[i].Include));
                }
                for (int i = 0; i < filters.itemGroup.ClIncludes.Count; i++)
                {
                    if (File.Exists(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(FileFilters), filters.itemGroup.ClIncludes[i].Include))))
                        filters.itemGroup.ClIncludes[i].Include = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(FileFilters), filters.itemGroup.ClIncludes[i].Include));
                }
            }
            filters.Save(FileFilters);
        }
        void BuildFilters()
        {
            string FiltersInProject = String.Empty;
            {
                var project = GenerateProjectFile.MapProjects[Name];
                string path = Path.GetDirectoryName(project.PathFileInfo);
                FiltersInProject = Path.Combine(path, Name + ".vcxproj.filters");
            }
            if (File.Exists(FileFilters)&&(!File.Exists(FiltersInProject)||File.GetLastWriteTime(FiltersInProject) > File.GetLastWriteTime(FileFilters)))
            {
                ReBuildFilters(FiltersInProject);
            }
            else
            { 
                BuildNewFilters();
            }
            
        }
        private static void GetFile(ref List<string> List, ref string Out)
        {
            switch (List.Count)
            {
                case 0:
                    Out = " УДАЛИТЬ.УДАЛИТЬ";
                    break;
                case 1:
                    Out = List[0];
                    break;
                default:
                    while (true)
                    {
                        Console.WriteLine("-------------------------------------------");
                        Console.WriteLine("Выберети файл для " + Out);
                        int d = 0;
                        foreach (string b in List)
                        {
                            Console.WriteLine(d + "-" + b);
                            d++;
                        }
                        Console.WriteLine("-------------------------------------------");
                        Console.Write(">");
                        try
                        {
                            string in1 = Console.ReadLine();
                            int id = int.Parse(in1);
                            if (id >= 0 && id <= List.Count)
                            {
                                Out = List[id]; return;
                            }
                        }
                        catch { }
                    }

            }
        }
        public static void CreateFilters(string File, string Project)
        {
            GenerateProjectFile GenerateProjectFile = new GenerateProjectFile();
            GenerateProjectFile.RegisterProject(Project);
            var prject = GenerateProjectFile.MapProjects[Project];
            Filters.Project filetrs = new Filters.Project();
            if (!filetrs.Load(File)) Console.WriteLine("Ошибка не удалось загрузить " + File);
            foreach (Filters.ClCompile i in filetrs.itemGroup.ClCompilers)
            {
                List<string> list = new List<string>();
                foreach (string a in prject.SourceFile)
                {
                    if (Path.GetFileName(i.Include).ToLower() == Path.GetFileName(a).ToLower()) list.Add(a);
                }
                GetFile(ref list, ref i.Include);


            }
            for (int i = filetrs.itemGroup.ClCompilers.Count - 1; i >= 0; i--)
            {
                if (filetrs.itemGroup.ClCompilers[i].Include == " УДАЛИТЬ.УДАЛИТЬ")
                {
                    filetrs.itemGroup.ClCompilers.RemoveAt(i);
                }
            }
            foreach (Filters.ClInclude i in filetrs.itemGroup.ClIncludes)
            {
                List<string> list = new List<string>();
                foreach (string a in prject.IncludeFile.Keys)
                {
                    if (Path.GetFileName(i.Include).ToLower() == Path.GetFileName(a).ToLower()) list.Add(a);
                }
                GetFile(ref list, ref i.Include);
            }
            for (int i = filetrs.itemGroup.ClIncludes.Count - 1; i >= 0; i--)
            {
                if (filetrs.itemGroup.ClIncludes[i].Include == " УДАЛИТЬ.УДАЛИТЬ")
                {
                    filetrs.itemGroup.ClIncludes.RemoveAt(i);
                }
            }
            bool b1 = false, b2 = false;
            foreach (Filters.None i in filetrs.itemGroup.Nones)
            {
                if (Path.GetFileName(i.Include).ToLower() == Path.GetFileName(prject.PathFileInfo).ToLower())
                {
                    List<string> list = new List<string>();
                    list.Add(prject.PathFileInfo);
                    if (b1) continue;
                    GetFile(ref list, ref i.Include);
                    b1 = true;
                }
                else if (Path.GetFileName(i.Include).ToLower() == Path.GetFileName(prject.ResourceFile).ToLower())
                {
                    List<string> list = new List<string>();
                    list.Add(prject.ResourceFile);
                    if (b2) continue;
                    GetFile(ref list, ref i.Include);
                    b2 = true;
                }
                else
                {
                    i.Include = " УДАЛИТЬ.УДАЛИТЬ";
                }
            }
            for (int i = filetrs.itemGroup.Nones.Count - 1; i >= 0; i--)
            {
                if (filetrs.itemGroup.Nones[i].Include == " УДАЛИТЬ.УДАЛИТЬ")
                {
                    filetrs.itemGroup.Nones.RemoveAt(i);
                }
            }
            string FiltersInProject = String.Empty;
            {
                var project = GenerateProjectFile.MapProjects[Project];
                string path = Path.GetDirectoryName(project.PathFileInfo);
                FiltersInProject = Path.Combine(path, Project + ".vcxproj.filters");
            }
            {
                Dictionary<string, bool> paths = new Dictionary<string, bool>();
                foreach (Filters.ClCompile i in filetrs.itemGroup.ClCompilers)
                {
                    if (paths.ContainsKey(i.Filter) == false)
                    {
                        paths.Add(i.Filter, true);
                    }
                }
                foreach (Filters.ClInclude i in filetrs.itemGroup.ClIncludes)
                {
                    if (paths.ContainsKey(i.Filter) == false)
                    {
                        paths.Add(i.Filter, true);
                    }
                }
                foreach (Filters.None i in filetrs.itemGroup.Nones)
                {
                    if (paths.ContainsKey(i.Filter) == false)
                    {
                        paths.Add(i.Filter, true);
                    }
                }
                foreach (Filters.Filter i in filetrs.itemGroup.Filters)
                {
                    if (paths.ContainsKey(i.Include) == true)
                    {
                        int offset = i.Include.LastIndexOf('\\');
                        if (offset == -1) continue;
                        string in_path = i.Include.Remove(offset);

                        if (paths.ContainsKey(in_path) == false)
                        {
                            paths.Add(in_path, true);
                        }
                    }
                }
                for (int i = filetrs.itemGroup.Filters.Count - 1; i >= 0; i--)
                {
                    if (paths.ContainsKey(filetrs.itemGroup.Filters[i].Include) == false)
                    {
                        filetrs.itemGroup.Filters.RemoveAt(i);
                    }
                }
            }
            filetrs.Save(FiltersInProject);

        }
    }
}