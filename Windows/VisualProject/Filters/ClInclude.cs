using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Filters
{
    class ClInclude:XmlObject
    {
        public string Include;
        public string Filter;

     
        public override string GetName()
        {
            return "ClInclude";
        }
        public override XmlObject Create()
        {
            return new ClInclude();
        }
        override protected bool LoadImpl( ref XmlNode parent)
        {
            if (!HasAttribute(ref parent, "Include"))
                return false;
            Include = GetAttribute(ref parent, "Include").Value;
            if (!HasNode(ref parent, "Filter"))
                return false;
            Filter = GetNode(ref parent, "Filter").InnerText;
            return true;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            AppendAttribute(ref xmlDocument, ref parent, "Include", Include);
            AppendNode(ref xmlDocument, ref parent, "Filter", Filter);

        }
    }
}
