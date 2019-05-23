using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BearBuildTool.Windows.VisualProject.Filters
{
    class None:ClInclude
    {
        public override string GetName()
        {
            return "None";
        }

        public override XmlObject Create()
        {
            return new None();
        }
    }
}
