using BearBuildTool.Projects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BearBuildTool.Windows
{
    class VCProjectFile
    {

        GenerateProjectFile GenerateProjectFile;
        string Name;
        public string File;
        public Guid Guid;
        public VCProjectFile(string name)
        {
            GenerateProjectFile = new GenerateProjectFile();
            GenerateProjectFile.RegisterProject(name);
            Name = name;
            Guid = Guid.NewGuid();
        }
        
        public void Write()
        {
            string LIntermediate = Path.Combine(Config.Global.IntermediatePath, "VCProjects");
            if (!Directory.Exists(LIntermediate))
            {
                Directory.CreateDirectory(LIntermediate);
            }
            File = Path.Combine(LIntermediate, Name + ".vcxproj");
            string FileFilters = File + ".filters";
            string FileUser = File + ".user";
            string command = "";
            if (Config.Global.ANSI)
            {
                command += "-ansi ";
            }
            if (Config.Global.WithoutWarning)
            {
                command += "-withoutwarning ";
            }
            List<string> ListLine = new List<string>();
            List<string> FiltersListLine = new List<string>();
            List<string> FileUserListLine = new List<string>();
            FileUserListLine.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            

 
             FiltersListLine.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            FiltersListLine.Add("<Project ToolsVersion=\"4.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
            FiltersListLine.Add("<ItemGroup>");
            ListLine.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            ListLine.Add("<Project DefaultTargets=\"Build\" ToolsVersion=\"14.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
            ListLine.Add("  <ItemGroup Label=\"ProjectConfigurations\">");
            ListLine.Add("    <ProjectConfiguration Include=\"Debug|Win32\">");
            ListLine.Add("      <Configuration>Debug</Configuration>");
            ListLine.Add("      <Platform>Win32</Platform>");
            ListLine.Add("    </ProjectConfiguration>");
            ListLine.Add("    <ProjectConfiguration Include=\"Mixed|Win32\">");
            ListLine.Add("      <Configuration>Mixed</Configuration>");
            ListLine.Add("      <Platform>Win32</Platform>");
            ListLine.Add("    </ProjectConfiguration>");
            ListLine.Add("    <ProjectConfiguration Include=\"Mixed|x64\">");
            ListLine.Add("      <Configuration>Mixed</Configuration>");
            ListLine.Add("      <Platform>x64</Platform>");
            ListLine.Add("    </ProjectConfiguration>");
            ListLine.Add("    <ProjectConfiguration Include=\"Release|Win32\">");
            ListLine.Add("      <Configuration>Release</Configuration>");
            ListLine.Add("      <Platform>Win32</Platform>");
            ListLine.Add("    </ProjectConfiguration>");
            ListLine.Add("    <ProjectConfiguration Include=\"Debug|x64\">");
            ListLine.Add("      <Configuration>Debug</Configuration>");
            ListLine.Add("      <Platform>x64</Platform>");
            ListLine.Add("    </ProjectConfiguration>");
            ListLine.Add("    <ProjectConfiguration Include=\"Release|x64\">");
            ListLine.Add("      <Configuration>Release</Configuration>");
            ListLine.Add("      <Platform>x64</Platform>");
            ListLine.Add("    </ProjectConfiguration>");
            ListLine.Add("  </ItemGroup>");
            ListLine.Add("  <ItemGroup>");
            string[] conditions = {
                "'$(Configuration)|$(Platform)'=='Debug|Win32'",
                "'$(Configuration)|$(Platform)'=='Mixed|Win32'",
                 "'$(Configuration)|$(Platform)'=='Release|Win32'",
                "'$(Configuration)|$(Platform)'=='Debug|x64'",
                "'$(Configuration)|$(Platform)'=='Mixed|x64'",
                "'$(Configuration)|$(Platform)'=='Release|x64'"
            };
            FileUserListLine.Add("<Project ToolsVersion=\"14.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
            for (int ii = 0; ii < 3; ii++)
            {
                FileUserListLine.Add(String.Format("  <PropertyGroup Condition=\"{0}\">", conditions[ii]));
                FileUserListLine.Add(String.Format("    <LocalDebuggerWorkingDirectory>..\\..\\binaries\\{0}\\</LocalDebuggerWorkingDirectory>", "win32"));
                FileUserListLine.Add("    <DebuggerFlavor>WindowsLocalDebugger</DebuggerFlavor>");
                FileUserListLine.Add("  </PropertyGroup>");
            };
            for (int ii = 3; ii < 6; ii++)
            {
                FileUserListLine.Add(String.Format("  <PropertyGroup Condition=\"{0}\">", conditions[ii]));
                FileUserListLine.Add(String.Format("    <LocalDebuggerWorkingDirectory>..\\..\\binaries\\{0}\\</LocalDebuggerWorkingDirectory>", "win64"));
                FileUserListLine.Add("    <DebuggerFlavor>WindowsLocalDebugger</DebuggerFlavor>");
                FileUserListLine.Add("  </PropertyGroup>");
            };
            FileUserListLine.Add("</Project>");
            for (int ii = 0; ii < GenerateProjectFile.MapProjects.Count; ii++)
            {
                var i = GenerateProjectFile.MapProjects.ElementAt(ii).Value;
                string pp = GenerateProjectFile.MapProjects.ElementAt(ii).Key;
                FiltersListLine.Add(String.Format("		<Filter Include=\"{0}\">", pp));
                FiltersListLine.Add(String.Format("			<UniqueIdentifier>{0}</UniqueIdentifier>", Guid.NewGuid().ToString("B")));
                FiltersListLine.Add("		</Filter>");
                FiltersListLine.Add(String.Format("		<Filter Include=\"{0}\\include\">", pp));
                FiltersListLine.Add(String.Format("			<UniqueIdentifier>{0}</UniqueIdentifier>", Guid.NewGuid().ToString("B")));
                FiltersListLine.Add("		</Filter>");
                FiltersListLine.Add(String.Format("		<Filter Include=\"{0}\\source\">", pp));
                FiltersListLine.Add(String.Format("			<UniqueIdentifier>{0}</UniqueIdentifier>", Guid.NewGuid().ToString("B")));
                FiltersListLine.Add("		</Filter>");
                foreach (var ji in i.SourceFile)
                {
                    FiltersListLine.Add(String.Format("		<ClCompile Include=\"{0}\">", ji));
                    FiltersListLine.Add(String.Format("		  <Filter>{0}\\source</Filter>", pp));
                    FiltersListLine.Add("		</ClCompile >");
                    ListLine.Add(String.Format("    <ClCompile Include=\"{0}\">", ji));

                    List<string> LInclude = i.Include.ToList();
                    List<string> LDefines = i.Defines.ToList();
                    string SInclude = "    <AdditionalIncludeDirectories Condition=\"{0}\">";
                    foreach (var ia in LInclude)
                    {
                        SInclude += ia + ";";
                    }
                    SInclude += "</AdditionalIncludeDirectories>";
                    string SDefines = "    <PreprocessorDefinitions Condition=\"{0}\">$(NMakePreprocessorDefinitions);";
                    foreach (var ia in LDefines)
                    {
                        SDefines += ia + ";";
                    }
                    SDefines += "</PreprocessorDefinitions>";
                    foreach (string ic in conditions)
                    {
                        ListLine.Add(String.Format(SDefines, ic));
                    }
                    foreach (string ic in conditions)
                    {
                        ListLine.Add(String.Format(SInclude, ic));
                    }
                    ListLine.Add(String.Format("    </ClCompile>"));
                }
            }
            ListLine.Add("  </ItemGroup>");
            ListLine.Add("  <ItemGroup>");
            for (int ii = 0; ii < GenerateProjectFile.MapProjects.Count; ii++)
            {
                var i = GenerateProjectFile.MapProjects.ElementAt(ii).Value;
                string pp = GenerateProjectFile.MapProjects.ElementAt(ii).Key;
                foreach (var ji in i.IncludeFile.Keys)
                {
                    FiltersListLine.Add(String.Format("		<ClInclude Include=\"{0}\">", ji));
                    FiltersListLine.Add(String.Format("		  <Filter>{0}\\include</Filter>", pp));
                    FiltersListLine.Add("		</ClInclude >");
                    ListLine.Add(String.Format("    <ClInclude Include=\"{0}\" />", ji));
                }
            }
            ListLine.Add("  </ItemGroup>");
            ListLine.Add("  <ItemGroup>");
            for (int ii = 0; ii < GenerateProjectFile.MapProjects.Count; ii++)
            {
                
                var i = GenerateProjectFile.MapProjects.ElementAt(ii).Value;
                string pp = GenerateProjectFile.MapProjects.ElementAt(ii).Key;
                FiltersListLine.Add(String.Format("		<None Include=\"{0}\">", i.NameFileInfo));
                FiltersListLine.Add(String.Format("		  <Filter>{0}</Filter>", pp));
                FiltersListLine.Add("		</None >");
                ListLine.Add(String.Format("    <None Include=\"{0}\" />", i.NameFileInfo));

            }
            ListLine.Add("  </ItemGroup>");

            ListLine.Add("  <PropertyGroup Label=\"Globals\">");
            ListLine.Add(String.Format("    <ProjectGuid>{0}</ProjectGuid>", Guid.ToString("B") ));
            ListLine.Add("    <Keyword>Win32Proj</Keyword>");
            ListLine.Add(String.Format("   <RootNamespace>{0}</RootNamespace>",Name));
            ListLine.Add("  </PropertyGroup>");
            ListLine.Add("  <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.Default.props\" />");
            ListLine.Add("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Debug|Win32'\" Label=\"Configuration\">");
            ListLine.Add("    <ConfigurationType>Makefile</ConfigurationType>");
            ListLine.Add("    <UseDebugLibraries>true</UseDebugLibraries>");
            ListLine.Add("    <PlatformToolset>v141</PlatformToolset>");
            ListLine.Add("  </PropertyGroup>");
            ListLine.Add("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Mixed|Win32'\" Label=\"Configuration\">");
            ListLine.Add("    <ConfigurationType>Makefile</ConfigurationType>");
            ListLine.Add("    <UseDebugLibraries>true</UseDebugLibraries>");
            ListLine.Add("    <PlatformToolset>v141</PlatformToolset>");
            ListLine.Add("  </PropertyGroup>");
            ListLine.Add("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Release|Win32'\" Label=\"Configuration\">");
            ListLine.Add("    <ConfigurationType>Makefile</ConfigurationType>");
            ListLine.Add("    <UseDebugLibraries>false</UseDebugLibraries>");
            ListLine.Add("    <PlatformToolset>v141</PlatformToolset>");
            ListLine.Add("  </PropertyGroup>");
            ListLine.Add("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Debug|x64'\" Label=\"Configuration\">");
            ListLine.Add("    <ConfigurationType>Makefile</ConfigurationType>");
            ListLine.Add("    <UseDebugLibraries>true</UseDebugLibraries>");
            ListLine.Add("    <PlatformToolset>v141</PlatformToolset>");
            ListLine.Add("  </PropertyGroup>");
            ListLine.Add("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Mixed|x64'\" Label=\"Configuration\">");
            ListLine.Add("    <ConfigurationType>Makefile</ConfigurationType>");
            ListLine.Add("    <UseDebugLibraries>true</UseDebugLibraries>");
            ListLine.Add("    <PlatformToolset>v141</PlatformToolset>");
            ListLine.Add("  </PropertyGroup>");
            ListLine.Add("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Release|x64'\" Label=\"Configuration\">");
            ListLine.Add("    <ConfigurationType>Makefile</ConfigurationType>");
            ListLine.Add("    <UseDebugLibraries>false</UseDebugLibraries>");
            ListLine.Add("    <PlatformToolset>v141</PlatformToolset>");
            ListLine.Add("  </PropertyGroup>");
            ListLine.Add("  <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.props\" />");
            ListLine.Add("  <ImportGroup Label=\"ExtensionSettings\">");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("  <ImportGroup Label=\"Shared\">");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("  <ImportGroup Label=\"PropertySheets\" Condition=\"'$(Configuration)|$(Platform)'=='Debug|Win32'\">");
            ListLine.Add("    <Import Project=\"$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props\" Condition=\"exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')\" Label=\"LocalAppDataPlatform\" />");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("  <ImportGroup Condition=\"'$(Configuration)|$(Platform)'=='Mixed|Win32'\" Label=\"PropertySheets\">");
            ListLine.Add("    <Import Project=\"$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props\" Condition=\"exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')\" Label=\"LocalAppDataPlatform\" />");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("  <ImportGroup Label=\"PropertySheets\" Condition=\"'$(Configuration)|$(Platform)'=='Release|Win32'\">");
            ListLine.Add("    <Import Project=\"$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props\" Condition=\"exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')\" Label=\"LocalAppDataPlatform\" />");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("  <ImportGroup Label=\"PropertySheets\" Condition=\"'$(Configuration)|$(Platform)'=='Debug|x64'\">");
            ListLine.Add("    <Import Project=\"$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props\" Condition=\"exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')\" Label=\"LocalAppDataPlatform\" />");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("  <ImportGroup Condition=\"'$(Configuration)|$(Platform)'=='Mixed|x64'\" Label=\"PropertySheets\">");
            ListLine.Add("    <Import Project=\"$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props\" Condition=\"exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')\" Label=\"LocalAppDataPlatform\" />");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("  <ImportGroup Label=\"PropertySheets\" Condition=\"'$(Configuration)|$(Platform)'=='Release|x64'\">");
            ListLine.Add("    <Import Project=\"$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props\" Condition=\"exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')\" Label=\"LocalAppDataPlatform\" />");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("  <PropertyGroup Label=\"UserMacros\" />");
            ListLine.Add("  <ItemDefinitionGroup/>");
            {
                string defines = "_DEBUG;DEBUG;WIN32;X32;WINDOWS;LIB;_LIB";
                if (!Config.Global.ANSI)
                {
                    defines += ";_UNICODE;";
                    defines += "UNICODE";
                }
                ListLine.Add(String.Format("  <PropertyGroup Condition=\"{0}\">", conditions[0]));
                ListLine.Add(String.Format("    <NMakeOutput>..\\..\\binaries\\{0}\\{1}{2}.exe</NMakeOutput>","win32",Name,"_debug"));
                ListLine.Add(String.Format("    <NMakePreprocessorDefinitions>{0}</NMakePreprocessorDefinitions>", defines));
                ListLine.Add("    <NMakeIncludeSearchPath></NMakeIncludeSearchPath>");
       
                ListLine.Add(String.Format("    <OutDir>..\\..\\binaries\\{0}\\</OutDir>","win32"));
                ListLine.Add("    <IntDir>..\\..\\binaries\\net\\</IntDir>");
                ListLine.Add(String.Format("    <NMakeBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} {0} {2} {1}</NMakeBuildCommandLine>", Name, "Win32", "Debug", command));
                ListLine.Add(String.Format("    <NMakeReBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe  {3} -rebuild {0} {2} {1}</NMakeReBuildCommandLine>", Name, "Win32", "Debug", command));
                ListLine.Add(String.Format("    <NMakeCleanCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe  {3} -clean {0} {2} {1}</NMakeCleanCommandLine>", Name, "Win32", "Debug", command));
               
                ListLine.Add("  </PropertyGroup>");

            }
            {
                string defines = "MIXED;DEBUG;WIN32;X32;WINDOWS;LIB;_LIB";
                if (!Config.Global.ANSI)
                {
                    defines += ";_UNICODE;";
                    defines += "UNICODE";
                }
                ListLine.Add(String.Format("  <PropertyGroup Condition=\"{0}\">", conditions[1]));
                ListLine.Add(String.Format("    <NMakeOutput>..\\..\\binaries\\{0}\\{1}{2}.exe</NMakeOutput>", "win32", Name, "_mixed"));
                ListLine.Add(String.Format("    <NMakePreprocessorDefinitions>{0}</NMakePreprocessorDefinitions>", defines));
                ListLine.Add("    <NMakeIncludeSearchPath></NMakeIncludeSearchPath>");

                ListLine.Add(String.Format("    <OutDir>..\\..\\binaries\\{0}\\</OutDir>", "win32"));
                ListLine.Add("    <IntDir>..\\..\\binaries\\net\\</IntDir>");
                ListLine.Add(String.Format("    <NMakeBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} {0} {2} {1}</NMakeBuildCommandLine>", Name,"Win32","Mixed", command));
                ListLine.Add(String.Format("    <NMakeReBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -rebuild  {0} {2} {1}</NMakeReBuildCommandLine>", Name, "Win32", "Mixed", command));
                ListLine.Add(String.Format("    <NMakeCleanCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -clean {0} {2} {1}</NMakeCleanCommandLine>", Name, "Win32", "Mixed", command));
                ListLine.Add("  </PropertyGroup>");
            }
            {
                string defines = "NDEBUG;WIN32;X32;WINDOWS;LIB;_LIB";
                if (!Config.Global.ANSI)
                {
                    defines += ";_UNICODE;";
                    defines += "UNICODE";
                }
                ListLine.Add(String.Format("  <PropertyGroup Condition=\"{0}\">", conditions[2]));
                ListLine.Add(String.Format("    <NMakeOutput>..\\..\\binaries\\{0}\\{1}{2}.exe</NMakeOutput>", "win32", Name, ""));
                ListLine.Add(String.Format("    <NMakePreprocessorDefinitions>{0}</NMakePreprocessorDefinitions>", defines));
                ListLine.Add("    <NMakeIncludeSearchPath></NMakeIncludeSearchPath>");

                ListLine.Add(String.Format("    <OutDir>..\\..\\binaries\\{0}\\</OutDir>", "win32"));
                ListLine.Add("    <IntDir>..\\..\\binaries\\net\\</IntDir>");
                ListLine.Add(String.Format("    <NMakeBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} {0} {2} {1}</NMakeBuildCommandLine>", Name, "Win32", "Release", command));
                ListLine.Add(String.Format("    <NMakeReBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -rebuild {0} {2} {1}</NMakeReBuildCommandLine>", Name, "Win32", "Release", command));
                ListLine.Add(String.Format("    <NMakeCleanCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -clean {0} {2} {1}</NMakeCleanCommandLine>", Name, "Win32", "Release", command));
                ListLine.Add("  </PropertyGroup>");

            }
            {
                string defines = "_DEBUG;DEBUG;X64;WINDOWS;LIB;_LIB";
                if (!Config.Global.ANSI)
                {
                    defines += ";_UNICODE;";
                    defines += "UNICODE";
                }
                ListLine.Add(String.Format("  <PropertyGroup Condition=\"{0}\">", conditions[3]));
                ListLine.Add(String.Format("    <NMakeOutput>..\\..\\binaries\\{0}\\{1}{2}.exe</NMakeOutput>", "win64", Name, "_debug"));
                ListLine.Add(String.Format("    <NMakePreprocessorDefinitions>{0}</NMakePreprocessorDefinitions>", defines));
                ListLine.Add("    <NMakeIncludeSearchPath></NMakeIncludeSearchPath>");

                ListLine.Add(String.Format("    <OutDir>..\\..\\binaries\\{0}\\</OutDir>", "win64"));
                ListLine.Add("    <IntDir>..\\..\\binaries\\net\\</IntDir>");
                ListLine.Add(String.Format("    <NMakeBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} {0} {2} {1}</NMakeBuildCommandLine>", Name, "Win64", "Debug", command));
                ListLine.Add(String.Format("    <NMakeReBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -rebuild {0} {2} {1}</NMakeReBuildCommandLine>", Name, "Win64", "Debug", command));
                ListLine.Add(String.Format("    <NMakeCleanCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -clean {0} {2} {1}</NMakeCleanCommandLine>", Name, "Win64", "Debug", command));
                ListLine.Add("  </PropertyGroup>");
            }
            {
                string defines = "MIXED;DEBUG;X64;WINDOWS;LIB;_LIB";
                if (!Config.Global.ANSI)
                {
                    defines += ";_UNICODE;";
                    defines += "UNICODE";
                }
                ListLine.Add(String.Format("  <PropertyGroup Condition=\"{0}\">", conditions[4]));
                ListLine.Add(String.Format("    <NMakeOutput>..\\..\\binaries\\{0}\\{1}{2}.exe</NMakeOutput>", "win64", Name, "_mixed"));
                ListLine.Add(String.Format("    <NMakePreprocessorDefinitions>{0}</NMakePreprocessorDefinitions>", defines));
                ListLine.Add("    <NMakeIncludeSearchPath></NMakeIncludeSearchPath>");

                ListLine.Add(String.Format("    <OutDir>..\\..\\binaries\\{0}\\</OutDir>", "win64"));
                ListLine.Add("    <IntDir>..\\..\\binaries\\net\\</IntDir>");
                ListLine.Add(String.Format("    <NMakeBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} {0} {2} {1}</NMakeBuildCommandLine>", Name, "Win64", "Mixed", command));
                ListLine.Add(String.Format("    <NMakeReBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -rebuild {0} {2} {1}</NMakeReBuildCommandLine>", Name, "Win64", "Mixed", command));
                ListLine.Add(String.Format("    <NMakeCleanCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -clean {0} {2} {1}</NMakeCleanCommandLine>", Name, "Win64", "Mixed", command));
                ListLine.Add("  </PropertyGroup>");
            }
            {
                string defines = "NDEBUG;X64;WINDOWS;LIB;_LIB";
                if (!Config.Global.ANSI)
                {
                    defines += ";_UNICODE;";
                    defines += "UNICODE";
                }
                ListLine.Add(String.Format("  <PropertyGroup Condition=\"{0}\">", conditions[5]));
                ListLine.Add(String.Format("    <NMakeOutput>..\\..\\binaries\\{0}\\{1}{2}.exe</NMakeOutput>", "win64", Name, ""));
                ListLine.Add(String.Format("    <NMakePreprocessorDefinitions>{0}</NMakePreprocessorDefinitions>", defines));

                ListLine.Add("    <NMakeIncludeSearchPath></NMakeIncludeSearchPath>");

                ListLine.Add(String.Format("    <OutDir>..\\..\\binaries\\{0}\\</OutDir>", "win64"));
                ListLine.Add("    <IntDir>..\\..\\binaries\\net\\</IntDir>");
                ListLine.Add(String.Format("    <NMakeBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} {0} {2} {1}</NMakeBuildCommandLine>", Name, "Win64", "Release", command));
                ListLine.Add(String.Format("    <NMakeReBuildCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -rebuild {0} {2} {1}</NMakeReBuildCommandLine>", Name, "Win64", "Release", command));
                ListLine.Add(String.Format("    <NMakeCleanCommandLine>..\\..\\binaries\\net\\BearBuildTool.exe {3} -clean {0} {2} {1}</NMakeCleanCommandLine>", Name, "Win64", "Release", command));
                ListLine.Add("  </PropertyGroup>");

            }
               
            ListLine.Add("  <Import Project=\"$(VCTargetsPath)\\Microsoft.Cpp.targets\" />");
            ListLine.Add("  <ImportGroup Label=\"ExtensionTargets\">");
            ListLine.Add("  </ImportGroup>");
            ListLine.Add("</Project>");
            FiltersListLine.Add("</ItemGroup>");
            FiltersListLine.Add("</Project>");
            System.IO.File.WriteAllLines(FileFilters, FiltersListLine);
            System.IO.File.WriteAllLines(File, ListLine);
            System.IO.File.WriteAllLines(FileUser, FileUserListLine);
        }
    }
}
