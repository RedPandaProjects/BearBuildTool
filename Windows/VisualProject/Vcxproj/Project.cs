using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Vcxproj
{
    class Project : XmlProject
    {
        public string DefaultTargets = "Build";
        public List<PropertyGroup> propertyGroups = new List<PropertyGroup>();
        public List<Import> imports = new List<Import>();
        public List<ImportGroup> importGroups = new List<ImportGroup>();
        public List<ItemGroup> itemGroups = new List<ItemGroup>();
        public Project()
        {
            ToolsVersion = "14.0";
        }
        public override XmlObject Create()
        {
            return new Project();
        }

        override protected bool LoadImpl(ref XmlNode parent)
        {
            /*  if (HasAttribute(ref parent, "DefaultTargets"))
                  DefaultTargets = GetAttribute(ref parent, "DefaultTargets").Value;
              return base.LoadImpl(ref parent);*/
            return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            AppendAttribute(ref xmlDocument, ref parent, "DefaultTargets", DefaultTargets);
            Save(ref itemGroups, ref xmlDocument, ref parent);
            foreach (PropertyGroup i in propertyGroups)
            {
                if(i.Label==PropertyGroup.ELabel.EL_Globals)
                {
                    i.Save(ref xmlDocument, ref parent);
                }
            }
            (new Import("$(VCTargetsPath)\\Microsoft.Cpp.Default.props")).Save(ref xmlDocument, ref parent);
            foreach (PropertyGroup i in propertyGroups)
            {
                if (i.Label == PropertyGroup.ELabel.EL_Configuration)
                {
                    i.Save(ref xmlDocument, ref parent);
                }
            }
            (new Import("$(VCTargetsPath)\\Microsoft.Cpp.props")).Save(ref xmlDocument, ref parent);
            foreach (ImportGroup i in importGroups)
            {
                if (i.Label == ImportGroup.ELabel.EL_ExtensionSettings)
                {
                    i.Save(ref xmlDocument, ref parent);
                }
            }
            foreach (ImportGroup i in importGroups)
            {
                if (i.Label == ImportGroup.ELabel.EL_Shared || i.Label == ImportGroup.ELabel.EL_PropertySheets)
                {
                    i.Save(ref xmlDocument, ref parent);
                }
            }
            foreach (PropertyGroup i in propertyGroups)
            {
                if (i.Label == PropertyGroup.ELabel.EL_UserMacros|| i.Label == PropertyGroup.ELabel.EL_Default)
                {
                    i.Save(ref xmlDocument, ref parent);
                }
            }
            (new Import("$(VCTargetsPath)\\Microsoft.Cpp.targets")).Save(ref xmlDocument, ref parent);
            foreach (ImportGroup i in importGroups)
            {
                if (i.Label == ImportGroup.ELabel.EL_ExtensionTargets)
                {
                    i.Save(ref xmlDocument, ref parent);
                }
            }

            base.SaveImpl(ref xmlDocument, ref parent);
        }
    }
}
