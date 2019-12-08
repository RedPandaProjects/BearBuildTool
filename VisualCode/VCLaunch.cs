using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearBuildTool.VisualCode
{
    class VCLaunch
    {
        public string version = "0.2.0";
        public class Configurate
        {
            public string name;
            public string type = "cppdbg";
            public string request = "launch";
            public string program;
    
            public IList<string> args = new List<string>();
            public bool stopAtEntry = false;
            public string cwd;
            public class Environment
            {
                public string Name = null;
                public string Value = null;
            }
            public IList<Environment> environment = new List<Environment>();
          
            public bool externalConsole = true;
            public string MIMode = "gdb";
            public string miDebuggerPath = null;
            public class SetupCommand
            {
                public string description = null;
                public string text = null;
                public bool ignoreFailures = false;
            }
            public IList<SetupCommand> setupCommands = new List<SetupCommand>();
            public string preLaunchTask = null;
        }
        public IList<Configurate> configurations = new List<Configurate>();

    }
}
