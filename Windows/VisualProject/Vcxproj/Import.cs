using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Vcxproj
{
    class Import : XmlObject
    {
        public Import()
        {

        }
        public Import(string project)
        {
            Project = project;
        }
        public enum ELabel
        {
            EL_Default,
            EL_LocalAppDataPlatform,
        }
        public ELabel Label = ELabel.EL_Default;
        //Default
        public string Project;
        public override string GetName()
        {
            return "Import";
        }
        public override XmlObject Create()
        {
            return new Import();
        }
        override protected bool LoadImpl(ref XmlNode parent)
        {
          
            return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            switch (Label)
            {
                case ELabel.EL_Default:
                    AppendAttribute(ref xmlDocument, ref parent, "Project", Project);
                    break;
                case ELabel.EL_LocalAppDataPlatform:
                    AppendAttribute(ref xmlDocument, ref parent, "Label", "LocalAppDataPlatform");
                    AppendAttribute(ref xmlDocument, ref parent, "Project", "$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props");
                    AppendAttribute(ref xmlDocument, ref parent, "Condition", "exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')");
                    break;
            }
        }
    }
}
