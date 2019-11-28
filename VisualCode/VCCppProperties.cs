using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearBuildTool.VisualCode
{
    class VCCppProperties
    {
       public class Configurate
        {
            public string name;
            public IList<string> includePath;
            public IList<string> defines;
            public string compilerPath;
            public string cStandard="c11";
            public string cppStandard = "c++17";
            public string intelliSenseMode = "gcc-x64";
        }
        public IList<Configurate> configurations = new List<Configurate>();
        public  int version=4;
    }
}
