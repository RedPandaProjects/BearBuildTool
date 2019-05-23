using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Vcxproj
{
    class ItemGroup : XmlObject
    {
        public List<Files.ClCompile> clCompiles = new List<Files.ClCompile>();
        public List<Files.ClInclude> clIncludes = new List<Files.ClInclude>();
        public List<Files.None> nones = new List<Files.None>();
        public List<ProjectConfiguration> projectConfigurations = new List<ProjectConfiguration>();


        public override string GetName()
        {
            return "ItemGroup";
        }
        public override XmlObject Create()
        {
            return new ItemGroup();
        }
        override protected bool LoadImpl(ref XmlNode parent)
        {
          /*  clCompiles = LoadList<Files.ClCompile>(ref parent);
            clIncludes = LoadList<Files.ClInclude>(ref parent);
            nones = LoadList<Files.None>(ref parent);
            if (HasAttribute(ref parent, "Label"))
                if (GetAttribute(ref parent, "Label").Value == "ProjectConfigurations")
                    projectConfigurations = LoadList<ProjectConfiguration>(ref parent);

            return projectConfigurations.Count > 0 || clCompiles.Count > 0 || nones.Count > 0 || projectConfigurations.Count > 0;*/
         return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            if (projectConfigurations.Count > 0)
            {
                AppendAttribute(ref xmlDocument, ref parent, "Label", "ProjectConfigurations");
                Save(ref projectConfigurations, ref xmlDocument, ref parent);
            }
            Save(ref clCompiles, ref xmlDocument, ref parent);
            Save(ref clIncludes, ref xmlDocument, ref parent);
            Save(ref nones, ref xmlDocument, ref parent);
         
    }
    }
}
