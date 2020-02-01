using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BearBuildTool.Windows
{
    class VSProjectGenerate
    {
        List<string> SlnLineList=new List<string>();
        List<Guid> Guids=new List<Guid>();
    
        public void Generate(string name)
        {
            string SlnGUID = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
            string outFileSln = Path.Combine(Config.Global.IntermediatePath, "..", name+".sln");
            SlnLineList.Add("");
            SlnLineList.Add("Microsoft Visual Studio Solution File, Format Version 12.00");
    
            if(Config.Global.vs2019)
            {
                SlnLineList.Add("# Visual Studio Version 16");
                SlnLineList.Add("VisualStudioVersion = 16.0.29519.181");
                SlnLineList.Add("MinimumVisualStudioVersion = 10.0.40219.1");
            }
            else
            {
                SlnLineList.Add("# Visual Studio 15");
                SlnLineList.Add("VisualStudioVersion = 15.0.28010.2036");
                SlnLineList.Add("MinimumVisualStudioVersion = 10.0.40219.1");
            }


            List<string> projects = new List<string>();
            {
                Projects.GenerateProjectFile generateProjectFile = new Projects.GenerateProjectFile();
                generateProjectFile.GetProjects(name,ref projects,new Config.Platform[]{ Config.Platform.Win32, Config.Platform.Win64, Config.Platform.MinGW});
            }
            
            foreach(string i in projects)
            {
                VisualProject.VisualProject visualProject = new VisualProject.VisualProject(i,name);
                visualProject.Build();
                Guids.Add(visualProject.Guid);
                /* VSProjectFile projectFile = new VSProjectFile(i,name);
                 projectFile.Write();
               */
                SlnLineList.Add(String.Format("Project(\"{0}\")=\"{1}\",\"{2}\",\"{3}\"", SlnGUID, i, visualProject.FileVcxproj, visualProject.Guid.ToString("B").ToUpper()));
                SlnLineList.Add("EndProject");
            }
            SlnLineList.Add("Global");
            SlnLineList.Add("GlobalSection(SolutionConfigurationPlatforms) = preSolution");
            SlnLineList.Add("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
            SlnLineList.Add("\t\tDebug|x64 = Debug|x64");
            SlnLineList.Add("\t\tDebug|x86 = Debug|x86");
            SlnLineList.Add("\t\tMixed|x64 = Mixed|x64");
            SlnLineList.Add("\t\tMixed|x86 = Mixed|x86");
            SlnLineList.Add("\t\tRelease|x64 = Release|x64");
            SlnLineList.Add("\t\tRelease|x86 = Release|x86");
            SlnLineList.Add("\t\tDebug|MinGW = Debug|MinGW");
            SlnLineList.Add("\t\tMixed|MinGW = Mixed|MinGW");
            SlnLineList.Add("\t\tRelease|MinGW = Release|MinGW");
            SlnLineList.Add("\tEndGlobalSection");
            SlnLineList.Add("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            foreach (Guid i in Guids)
            {
                SlnLineList.Add(String.Format("\t\t{0}.Debug|x64.ActiveCfg = Debug|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Debug|x64.Build.0 = Debug|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Debug|x86.ActiveCfg = Debug|Win32", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Debug|x86.Build.0 = Debug|Win32", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Debug|MinGW.ActiveCfg = Debug_MinGW|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Debug|MinGW.Build.0 = Debug_MinGW|x64", i.ToString("B").ToUpper()));

                SlnLineList.Add(String.Format("\t\t{0}.Mixed|x64.ActiveCfg = Mixed|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|x64.Build.0 = Mixed|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|x86.ActiveCfg = Mixed|Win32", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|x86.Build.0 = Mixed|Win32", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|MinGW.ActiveCfg = Mixed_MinGW|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|MinGW.Build.0 = Mixed_MinGW|x64", i.ToString("B").ToUpper()));

                SlnLineList.Add(String.Format("\t\t{0}.Release|x64.ActiveCfg = Release|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Release|x64.Build.0 = Release|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Release|x86.ActiveCfg = Release|Win32", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Release|x86.Build.0 = Release|Win32", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Release|MinGW.ActiveCfg = Release_MinGW|x64", i.ToString("B").ToUpper()));
                SlnLineList.Add(String.Format("\t\t{0}.Release|MinGW.Build.0 = Release_MinGW|x64", i.ToString("B").ToUpper()));
            }
            SlnLineList.Add("\tEndGlobalSection");
            SlnLineList.Add("EndGlobal");
            File.WriteAllLines(outFileSln, SlnLineList);
        }

    }
}
