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
                public string Name;
                public string Value;
            }
            public IList<Environment> environment = new List<Environment>();
          
            public bool externalConsole = true;
            public string MIMode = "gdb";
            public string miDebuggerPath = null;
            public class SetupCommand
            {
                public string description;
                public string text;
                public bool ignoreFailures;
            }
            public IList<SetupCommand> setupCommands = new List<SetupCommand>();
            public string preLaunchTask = null;
        }
        public IList<Configurate> configurations = new List<Configurate>();

    }
}
