using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BearBuildTool.Windows.VisualProject.Vcxproj.Files
{
    class ClInclude:None
    {
        public override string GetName()
        {
            return "ClInclude";
        }
        public override XmlObject Create()
        {
            return new ClInclude();
        }
    }
}
