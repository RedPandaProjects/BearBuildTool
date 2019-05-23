using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.User
{
    class PropertyGroup : XmlObject
    {
        public string Configuration = "";
        public string Platform = "";
        public string LocalDebuggerWorkingDirectory = "";
        public string DebuggerFlavor = "WindowsLocalDebugger";
        public override string GetName()
        {
            return "PropertyGroup";
        }
        public override XmlObject Create()
        {
            return new PropertyGroup();
        }
        override protected bool LoadImpl(ref XmlNode parent)
        {
           /* if (!HasAttribute(ref parent, "Condition"))
                return false;
            Condition = GetAttribute(ref parent, "Condition").Value;
            if (!HasNode(ref parent, "LocalDebuggerWorkingDirectory"))
                return false;
            LocalDebuggerWorkingDirectory = GetNode(ref parent, "LocalDebuggerWorkingDirectory").InnerText;
            if (!HasNode(ref parent, "DebuggerFlavor"))
                return false;
            DebuggerFlavor = GetNode(ref parent, "DebuggerFlavor").InnerText;*/
            return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            AppendAttribute(ref xmlDocument, ref parent, "Condition", String.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", Configuration, Platform));
            AppendNode(ref xmlDocument, ref parent, "LocalDebuggerWorkingDirectory", LocalDebuggerWorkingDirectory);
            AppendNode(ref xmlDocument, ref parent, "DebuggerFlavor", DebuggerFlavor);

        }
    }
}
