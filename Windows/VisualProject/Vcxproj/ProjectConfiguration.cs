using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Vcxproj
{
    class ProjectConfiguration:XmlObject
    {
        public override string GetName()
        {
            return "ProjectConfiguration";
        }
        public override XmlObject Create()
        {
            return new ProjectConfiguration();
        }
        
        public string Configuration = "";
        public string Platform = "";
        public ProjectConfiguration(string configuration, string platform )
        {
            Configuration = configuration;
            Platform = platform;
        }
        public ProjectConfiguration()
        {

        }
        override protected bool LoadImpl(ref XmlNode parent)
        {
           /* if (!HasAttribute(ref parent, "Include"))
                return false;
            string Include = GetAttribute(ref parent, "Include").Value;
            if (!HasNode(ref parent, "Configuration") || !HasNode(ref parent, "Platform"))
                return false;
            Configuration = GetNode(ref parent, "Configuration").Value;
            Platform = GetNode(ref parent, "Platform").Value;
            if (Include == (Configuration + "|" + Platform))
                return true;*/
            return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            AppendAttribute(ref xmlDocument, ref parent, "Include", Configuration + "|" + Platform);
            AppendNode(ref xmlDocument, ref parent, "Configuration", Configuration);
            AppendNode(ref xmlDocument, ref parent, "Platform", Platform);
        }
    }
}
