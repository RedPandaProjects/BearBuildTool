using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BearBuildTool.VisualCode
{
    public class VCProjectGenerate
    {
        public VCProjectGenerate()
        {
        }
        public void Generate(string name)
        {
         
            List<string> projects = new List<string>();
            Projects.GenerateProjectFile generateProjectFile = new Projects.GenerateProjectFile();
            {
    
                generateProjectFile.GetProjects(name, ref projects, new Config.Platform[] { Config.Platform.MinGW });

            }
            string VisualCodePath = Path.Combine(Config.Global.IntermediatePath, "..","VisualCode");
            if (!Directory.Exists(VisualCodePath))
            {
                Directory.CreateDirectory(VisualCodePath);
            }
            string ProjectsDirectory = Path.Combine(VisualCodePath, name);
            if(!Directory.Exists(ProjectsDirectory))
            {
                Directory.CreateDirectory(ProjectsDirectory);
            }


            VCWorkspace vcWorkspace = new VCWorkspace();



            foreach (string i in projects)
            {
                VCProjectFile projectFile = new VCProjectFile(i, name);

                projectFile.Write();
                vcWorkspace.folders.Add(new VCWorkspace.Folder(Path.GetDirectoryName( projectFile.VCDirectory).Replace("\\", "/")));
            }

            File.WriteAllText(Path.Combine(VisualCodePath, name + ".code-workspace"), JsonConvert.SerializeObject(vcWorkspace, Formatting.Indented));
        }
    }

}