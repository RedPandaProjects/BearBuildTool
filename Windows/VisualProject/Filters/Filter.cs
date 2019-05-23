using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Filters
{
    class Filter:XmlObject
    {
        public string Include;
        public Guid UniqueIdentifier;

        public override string GetName()
        {
            return "Filter";
        }
        public override XmlObject Create()
        {
            return new Filter();
        }

        override protected bool LoadImpl(ref XmlNode parent)
        {
            if (!HasAttribute(ref parent, "Include"))
                return false;
            Include = GetAttribute(ref parent, "Include").Value;
            if (!HasNode(ref parent, "UniqueIdentifier"))
                return false;
            if (Guid.TryParseExact(GetNode(ref parent, "UniqueIdentifier").InnerText, "B", out UniqueIdentifier)==false)
                return false;
            return true;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            AppendAttribute(ref xmlDocument, ref parent, "Include", Include);
            AppendNode(ref xmlDocument, ref parent, "UniqueIdentifier", UniqueIdentifier.ToString("B"));
        }
    }
}
