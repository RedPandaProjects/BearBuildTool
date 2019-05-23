using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BearBuildTool.Windows.VisualProject;
namespace BearBuildTool.Windows.VisualProject.Filters
{
    class ClCompile:ClInclude
    {
        public override string GetName()
        {
            return "ClCompile";
        }

        public override XmlObject Create()
        {
            return new ClCompile();
        }
    }
}
