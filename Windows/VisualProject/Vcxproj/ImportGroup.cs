using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Vcxproj
{
    class ImportGroup:XmlObject
    {
        public enum ELabel
        {
            EL_ExtensionSettings,
            EL_Shared,
            EL_PropertySheets,
            EL_ExtensionTargets,
        }
        public ELabel Label = ELabel.EL_ExtensionSettings;
        //PropertySheets
        public string Configuration = "";
        public string Platform = "";

        public override string GetName()
        {
            return "ImportGroup";
        }
        public override XmlObject Create()
        {
            return new ImportGroup();
        }
        override protected bool LoadImpl(ref XmlNode parent)
        {

            return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            switch (Label)
            {
                case ELabel.EL_ExtensionSettings:
                    AppendAttribute(ref xmlDocument, ref parent, "Label", "ExtensionSettings");
                    break;
                case ELabel.EL_Shared:
                    AppendAttribute(ref xmlDocument, ref parent, "Label", "Shared");
                    break;
                case ELabel.EL_PropertySheets:
                    AppendAttribute(ref xmlDocument, ref parent, "Label", "ExtensionSettings");
                    AppendAttribute(ref xmlDocument, ref parent, "Condition", String.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", Configuration, Platform));
                    Import import = new Import();
                    import.Label = Import.ELabel.EL_LocalAppDataPlatform;
                    import.Save(ref xmlDocument, ref parent);
                    break;
                case ELabel.EL_ExtensionTargets:
                    AppendAttribute(ref xmlDocument, ref parent, "Label", "ExtensionTargets");
                    break;
            }
        }
    }
}
