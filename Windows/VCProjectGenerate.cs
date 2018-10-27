using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BearBuildTool.Windows
{
    class VCProjectGenerate
    {
        List<string> SlnLineList=new List<string>();
        List<Guid> Guids=new List<Guid>();
    
        public void Generate(List<string> projects)
        {
            string SlnGUID = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
            string outFileSln = Path.Combine(Config.Global.IntermediatePath, "..", "Bear.sln");
            SlnLineList.Add("");
            SlnLineList.Add("Microsoft Visual Studio Solution File, Format Version 12.00");
            SlnLineList.Add("# Visual Studio 15");
            SlnLineList.Add("VisualStudioVersion = 15.0.28010.2036");
            SlnLineList.Add("MinimumVisualStudioVersion = 10.0.40219.1");
            foreach(string i in projects)
            {
                VCProjectFile projectFile = new VCProjectFile(i);
                projectFile.Write();
                Guids.Add(projectFile.Guid);
                SlnLineList.Add(String.Format("Project(\"{0}\")=\"{1}\",\"{2}\",\"{3}\"", SlnGUID, i,projectFile.File,  projectFile.Guid.ToString("B")));
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
            SlnLineList.Add("\tEndGlobalSection");
            SlnLineList.Add("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
            foreach (Guid i in Guids)
            {
                SlnLineList.Add(String.Format("\t\t{0}.Debug|x64.ActiveCfg = Debug|x64", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Debug|x64.Build.0 = Debug|x64", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Debug|x86.ActiveCfg = Debug|Win32", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Debug|x86.Build.0 = Debug|Win32", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|x64.ActiveCfg = Mixed|x64", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|x64.Build.0 = Mixed|x64", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|x86.ActiveCfg = Mixed|Win32", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Mixed|x86.Build.0 = Mixed|Win32", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Release|x64.ActiveCfg = Release|x64", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Release|x64.Build.0 = Release|x64", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Release|x86.ActiveCfg = Release|Win32", i.ToString("B")));
                SlnLineList.Add(String.Format("\t\t{0}.Release|x86.Build.0 = Release|Win32", i.ToString("B")));
            }
            SlnLineList.Add("\tEndGlobalSection");
            SlnLineList.Add("EndGlobal");
            File.WriteAllLines(outFileSln, SlnLineList);
        }

    }
}
