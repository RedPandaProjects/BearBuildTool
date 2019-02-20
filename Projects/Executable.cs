using System;
using System.Collections.Generic;
using System.Text;

namespace BearBuildTool.Projects
{
    public abstract class Executable:Project
    {
        public bool Console;
        public Executable()
        {
            Console = false;
        }
    }
}
