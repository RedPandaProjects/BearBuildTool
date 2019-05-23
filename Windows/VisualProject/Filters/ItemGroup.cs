using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Filters
{
    class ItemGroup:XmlObject
    {
        public  List<Filter> Filters = new List<Filter>();
        public  List<ClInclude> ClIncludes = new List<ClInclude>();
        public  List<ClCompile> ClCompilers = new List<ClCompile>();
        public List<None> Nones = new List<None>();
        public void AppendFilter(string Nmae)
        {
            Filter filter = new Filter();
            filter.Include = Nmae;
            filter.UniqueIdentifier = Guid.NewGuid();
            Filters.Add(filter);
        }
        public  void AppendClInclude(string Path,string Filter)
        {
            ClInclude clInclude = new ClInclude();
            clInclude.Include = Path;
            clInclude.Filter = Filter;
            ClIncludes.Add(clInclude);
        }
        public void AppendClCompile(string Path, string Filter)
        {
            ClCompile clInclude = new ClCompile();
            clInclude.Include = Path;
            clInclude.Filter = Filter;
            ClIncludes.Add(clInclude);
        }
        public override string GetName()
        {
            return "ItemGroup";
        }
        public override XmlObject Create()
        {
            return new ItemGroup();
        }
        override protected bool LoadImpl(ref XmlNode parent)
        {
            Filters =(new Filter()).LoadList<Filter>(ref parent);
            ClIncludes = (new ClInclude()).LoadList<ClInclude>(ref parent);
            ClCompilers = (new ClCompile()).LoadList<ClCompile>(ref parent);
            Nones = LoadList<None>(ref parent);
            return true;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            Save<Filter>(ref Filters, ref xmlDocument,ref parent);
            Save<ClInclude>(ref ClIncludes, ref xmlDocument, ref parent);
            Save<ClCompile>(ref ClCompilers, ref xmlDocument, ref parent);
            Save<None>(ref Nones, ref xmlDocument, ref parent);
        }
    }
}
