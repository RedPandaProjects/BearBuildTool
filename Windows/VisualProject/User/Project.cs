using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.User
{
    class Project:XmlProject
    {
        public List<PropertyGroup> propertyGroups = new List<PropertyGroup>();
       /* public void  AppendPropertGroup(string Condition,string LocalDebuggerWorkingDirectory , string DebuggerFlavor  )
        {
            PropertyGroup propertyGroup = new PropertyGroup();
            propertyGroup.Condition = Condition;
            propertyGroup.LocalDebuggerWorkingDirectory = LocalDebuggerWorkingDirectory;
            propertyGroup.DebuggerFlavor = DebuggerFlavor;
            propertyGroups.Add(propertyGroup);
        }*/
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
            /*  propertyGroups=(new PropertyGroup()).LoadList<PropertyGroup>(ref parent);
              return base.LoadImpl(ref parent);*/
            return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            Save<PropertyGroup>(ref propertyGroups, ref xmlDocument, ref parent);
            base.SaveImpl(ref xmlDocument, ref parent);
        }
    }
}
