using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearBuildTool.VisualCode
{
    class VCTasks
    {
        public class Task
        {
            public string label;
            public string type = "shell";
            public string command;
            public IList<string> args = new List<string>();
        }
        public IList<Task> tasks = new List<Task>();
        public string version = "2.0.0";
    }
}
