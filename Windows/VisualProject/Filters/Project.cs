using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Filters
{
    class Project: XmlProject
    {
        public ItemGroup itemGroup =new ItemGroup();

        public override XmlObject Create()
        {
            return new Project();
        }
        
        override protected bool LoadImpl(ref XmlNode parent)
        {
            List<ItemGroup> itemGroups = LoadList<ItemGroup>( ref parent);
            foreach(ItemGroup group in itemGroups)
            {
                itemGroup.Filters.AddRange(group.Filters);
                itemGroup.ClCompilers.AddRange(group.ClCompilers);
                itemGroup.ClIncludes.AddRange(group.ClIncludes);
                itemGroup.Nones.AddRange(group.Nones);
            }
            return base.LoadImpl(ref parent);
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            itemGroup.Save(ref xmlDocument, ref parent);
            base.SaveImpl(ref xmlDocument, ref parent);
        }
    }
}
