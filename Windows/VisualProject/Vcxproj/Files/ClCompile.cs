using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BearBuildTool.Windows.VisualProject.Vcxproj.Files
{
    class ClCompile : None
    {
        public override string GetName()
        {
            return "ClCompile";
        }
        public override XmlObject Create()
        {
            return new ClInclude();
        }
    }
}
