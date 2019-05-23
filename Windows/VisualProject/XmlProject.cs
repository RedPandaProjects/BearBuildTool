using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject
{
    class XmlProject:XmlObject
    {
        public string ToolsVersion = "4.0";
        public string xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
        public override string GetName()
        {
            return "Project";
        }
        public override XmlObject Create()
        {
            return new XmlProject();
        }

        override protected bool LoadImpl(ref XmlNode parent)
        {

            if (!HasAttribute(ref parent, "ToolsVersion"))
                return false;
            if (GetAttribute(ref parent, "ToolsVersion").Value != ToolsVersion)
                return false;
            if (HasAttribute(ref parent, "xmlns")) xmlns = GetAttribute(ref parent, "xmlns").Value;
            return true;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {

            AppendAttribute(ref xmlDocument, ref parent, "ToolsVersion", ToolsVersion);
            AppendAttribute(ref xmlDocument, ref parent, "xmlns", xmlns);
        }
    }
}
