using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Vcxproj.Files
{
    class None:XmlObject
    {
        public override string GetName()
        {
            return "None";
        }
        public override XmlObject Create()
        {
            return new None();
        }
        public string Include = "";
        public None(string include)
        {
            Include = include;
        }
        public None()
        {
            Include = "";
        }
        override protected bool LoadImpl(ref XmlNode parent)
        {
            /* if (!HasAttribute(ref parent, "Include"))
                 return false;
             Include = GetAttribute(ref parent, "Include").Value;
             return true;*/
            return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            AppendAttribute(ref xmlDocument, ref parent, "Include", Include);

        }
    }
}
