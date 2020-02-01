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
        private static void GetRealPlatformAndConfiguration(ref string p,ref string c)
        {
            if (c == "Debug_MinGW")
            {
                c = "Debug";
                if (p == "Win32")
                    p = "MinGW32";
                else if (p == "Win64")
                    p = "MinGW";
            }
            else if (c == "Mixed_MinGW")
            {
                c = "Mixed";
                if (p == "Win32")
                    p = "MinGW32";
                else if (p == "Win64")
                    p = "MinGW";
            }
            else if (c == "Release_MinGW")
            {
                c = "WIN64";
                if (p == "Win32")
                    p = "MinGW32";
                else if (p == "Win64")
                    p = "MinGW";
            }
        }

       
        //this is fixed
        string[] Platfroms = { "Win32", "Win64" };
        string[] PlatfromsDefines = { "WIN32;X32;", "WIN64;X64;" };
        Config.Configure[] ConfigurationsType = { Config.Configure.Debug, Config.Configure.Mixed, Config.Configure.Release, Config.Configure.Debug, Config.Configure.Mixed, Config.Configure.Release };
        string[] Configurations = { "Debug", "Mixed", "Release", "Debug_MinGW", "Mixed_MinGW", "Release_MinGW" };
        string[] ConfigurationsDefines = { "DEBUG;_DEBUG;MSVC;", "MIXED;DEBUG;MSVC;", "NDEBUG;MSVC;", "DEBUG;_DEBUG;GCC;", "MIXED;DEBUG;GCC;", "NDEBUG;GCC;" };
        private Config.Platform GetRealPlatformAndConfiguration(string p, string c)
        {
            if (c == "Debug_MinGW")
            {
                c = "Debug";
                return Config.Platform.MinGW;
            }
            else if (c == "Mixed_MinGW")
            {
                c = "Mixed";
                return Config.Platform.MinGW;
            }
            else if (c == "Release_MinGW")
            {
                c = "WIN64";
                return Config.Platform.MinGW;
            }
            else if(p == Platfroms[0])
            {
                return Config.Platform.Win32;
            }
            else if (p == Platfroms[1])
            {
                return Config.Platform.Win32;
            }
            return Config.Platform.None;
        }
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
                CurrentFilters = Path.Combine(FullPath, CurrentFilters);
            }
            string ProjectsFilters = String.Empty;
            {
                var projectFileInfo = Config.Global.ProjectsCSFile[sub_name];
                string path = Path.GetDirectoryName(projectFileInfo);
                ProjectsFilters = Path.Combine(path, sub_name + ".vcxproj.filters");

            }
            try
            {
                File.Copy(CurrentFilters, ProjectsFilters, true);
            }
            catch { Console.WriteLine("Ошибка не удалось копировать {0} в {1}", CurrentFilters, ProjectsFilters); }
            finally
            {
               /* GenerateProjectFile LocalGenerateProjectFile = new GenerateProjectFile();
                LocalGenerateProjectFile.RegisterProject(name);
                var project = LocalGenerateProjectFile.MapProjects[sub_name];
                {
                    project.
                }*/
            }
        }

        private void AddFile()
        {
            {
                Vcxproj.ItemGroup itemGroup = new Vcxproj.ItemGroup();
                List<string> SourceFie = new List<string>();
                List<string> IncludeFile = new List<string>();

                GenerateProjectFile.GetSource(Name, ref SourceFie);
                GenerateProjectFile.GetIncludeFile(Name, ref IncludeFile);
                foreach (string p in SourceFie)
                {
                    Vcxproj.Files.ClCompile clCompile = new Vcxproj.Files.ClCompile();
                    clCompile.Include = p;
                    itemGroup.clCompiles.Add(clCompile);
                }
                foreach (string i in IncludeFile)
                {
                    Vcxproj.Files.ClInclude clInclude = new Vcxproj.Files.ClInclude();
                    clInclude.Include = i;
                    itemGroup.clIncludes.Add(clInclude);
                }
                {
                    Vcxproj.Files.None none = new Vcxproj.Files.None();
                    none.Include = GenerateProjectFile.PathFileInfo[Name];
                    itemGroup.nones.Add(none);
                }

                XmlVcxproj.itemGroups.Add(itemGroup);

            }
        }
        private void AddPropertyGroup()
        {
            string SInclude = "";
            {
                List<string> LInclude = new List<string>();
                GenerateProjectFile.GetInclude(Name, ref LInclude);// Project.Include.ToList();
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

      
            for (int i = 0; i < Platfroms.Length; i++)
            {

                string p = Platfroms[i];

                for (int a = 0; a < Configurations.Length; a++)
                {


                    string c = Configurations[a];
                    string defines = "WINDOWS;LIB;_LIB";
                    defines += PlatfromsDefines[i];
                    defines += ConfigurationsDefines[a];
                    List<string> LDefines = GenerateProjectFile.MapProjects[GetRealPlatformAndConfiguration(p,c)][ConfigurationsType[a]][Name].Defines.ToList();
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
                    GetRealPlatformAndConfiguration(ref p,ref c);
                    propertyGroup.Default.OutDir = String.Format("..\\..\\..\\binaries\\{0}\\", p);
                    propertyGroup.Default.IntDir = "..\\..\\..\\binaries\\net\\";
                    propertyGroup.Default.NMakePreprocessorDefinitions = defines;
                    propertyGroup.Default.NMakeIncludeSearchPath = SInclude;
                    propertyGroup.Default.NMakeOutput = String.Format("..\\..\\..\\binaries\\{0}\\{1}_{2}.exe", p, GeneralName, c.ToLower());
                    propertyGroup.Default.NMakeBuildCommandLine = String.Format("{4} {3} {0} {2} {1}", GeneralName, p, c, command, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName));
                    propertyGroup.Default.NMakeReBuildCommandLine = String.Format("{4} {3} -rebuild {0} {2} {1}", GeneralName, p, c, command, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName));
                    propertyGroup.Default.NMakeCleanCommandLine = String.Format("{4} {3} -clean {0} {2} {1}", GeneralName, p, c, command, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName));
                    XmlVcxproj.propertyGroups.Add(propertyGroup);
                }

            }

        }
        void BuildVcxproj()
        {
            InitVcxproj();
            AddFile();
            AddPropertyGroup();
            try { File.Delete(FileVcxproj); } catch { };
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
                    string _p = p, _c = c;
                    GetRealPlatformAndConfiguration(ref _p, ref _c);
                    propertyGroup.LocalDebuggerWorkingDirectory = String.Format("..\\..\\..\\binaries\\{0}", _p);
                    user.propertyGroups.Add(propertyGroup);
                }
            try { File.Delete(FileUser); } catch { };
            user.Save(FileUser);
        }
      
        void BuildNewFilters()
        {
            Filters.Project filters = new Filters.Project();
            filters.itemGroup.AppendFilter("include");
            filters.itemGroup.AppendFilter("source");

            List<string> SourceFie = new List<string>();
            List<string> IncludeFile = new List<string>();

            GenerateProjectFile.GetSource(Name, ref SourceFie);
            GenerateProjectFile.GetIncludeFile(Name, ref IncludeFile);



            foreach (var ji in SourceFie)
            {
                filters.itemGroup.AppendClCompile(ji, "source");
            }
            foreach (var ji in IncludeFile)
            {
                filters.itemGroup.AppendClInclude(ji, "include");
            }
            try { File.Delete(FileFilters); } catch { };
            filters.Save(FileFilters);
        }
        private void ReBuildFilters(string outfile)
        {

         /*   Filters.Project filters = new Filters.Project();
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
            filters.Save(FileFilters);*/
        }
        void BuildFilters()
        {
            string FiltersInProject = String.Empty;
            {
                string path = Path.GetDirectoryName(GenerateProjectFile.PathFileInfo[Name]);
                FiltersInProject = Path.Combine(path, Name + ".vcxproj.filters");
            }
            if(File.Exists(FiltersInProject))
            {
                CreateFilters(FiltersInProject, Name, Path.Combine(FullPath, Name + ".vcxproj.filters"));
                //ReBuildFilters(FiltersInProject);
                return;
            }
            BuildNewFilters();
            
        }

        private static int GetMaxPathEquals(string path1, string path2)
        {
            path1.Replace('/', '\\');
            path2.Replace('/', '\\');

            string[] paths1 = path1.Split('\\', '/');
            string[] paths2 = path2.Split('\\', '/');


            int PathCount = Math.Min(paths1.Length, paths2.Length);
            int delta1 = paths1.Length - PathCount;
            int delta2 = paths2.Length - PathCount;
            int Result = 0;
            for(int i= PathCount;i!=0;i--)
            {
                if(paths1[delta1 + i - 1] == paths2[delta2+i-1])
                {
                    Result++;
                }
                else
                {
                    break;
                }
            }
            return Result;
           
        }
        private static void GetFile(ref List<string> List, ref string Out)
        {
            int max = 0;
            if (List.Count > 1)
            {
                for (int x = 0; x < List.Count; x++)
                {
                    max = Math.Max(GetMaxPathEquals(List[x], Out), max);

                }
                for (int x = List.Count; x != 0; x--)
                {
                    if (GetMaxPathEquals(List[x - 1], Out) < max)
                    {
                        List.RemoveAt(x - 1);
                    }
                }
            }
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
        public static void CreateFilters(string File, string Project,string Out=null)
        {
            GenerateProjectFile GenerateProjectFile = new GenerateProjectFile();
            GenerateProjectFile.RegisterProject(Project);

            List<string> SourceFie = new List<string>();
            List<string> IncludeFile = new List<string>();
            List<string> ResourceFiles = new List<string>();


            GenerateProjectFile.GetResourceFile(Project, ref ResourceFiles);
            GenerateProjectFile.GetSource(Project, ref SourceFie);
            GenerateProjectFile.GetIncludeFile(Project, ref IncludeFile);
            
            Filters.Project filetrs = new Filters.Project();
            if (!filetrs.Load(File)) Console.WriteLine("Ошибка не удалось загрузить " + File);
            foreach (Filters.ClCompile i in filetrs.itemGroup.ClCompilers)
            {
                List<string> list = new List<string>();
                foreach (string a in SourceFie)
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
                foreach (string a in IncludeFile)
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
                if (Path.GetFileName(i.Include).ToLower() == Path.GetFileName(GenerateProjectFile.PathFileInfo[Project]).ToLower())
                {
                    List<string> list = new List<string>();
                    list.Add(GenerateProjectFile.PathFileInfo[Project]);
                    if (b1) continue;
                    GetFile(ref list, ref i.Include);
                    b1 = true;
                }
                else
                if(ResourceFiles.Count!=0)
                {
                    foreach (string a in ResourceFiles)
                    {
                        if (Path.GetFileName(i.Include).ToLower() == Path.GetFileName(a).ToLower())
                        {
                            List<string> list = new List<string>();
                            list.Add(a);
                            if (b2) continue;
                            GetFile(ref list, ref i.Include);
                            b2 = true;
                        }
                        else
                        {
                            i.Include = " УДАЛИТЬ.УДАЛИТЬ";
                        }
                    }

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
                string path = Path.GetDirectoryName(GenerateProjectFile.PathFileInfo[Project]);
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
            filetrs.Save(Out != null? Out: FiltersInProject);

        }
    }
}