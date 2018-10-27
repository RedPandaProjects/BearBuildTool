using BearBuildTool.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BearBuildTool.Tools
{
    public class BuildTools
    {
        public virtual void BuildObject(List<string> LInclude, List<string> LDefines, string pch,string pchH,bool createPCH, string source, string obj,BuildType buildType)
        {

        }
        public virtual void BuildDynamicLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outDynamicLib, string outStaticLib)
        {

        }
        public virtual void BuildExecutable(List<string> objs, List<string> libs, List<string> libsPath, string Executable, string outStaticLib, bool Console)
        {

        }
        public virtual void BuildStaticLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outStaticLib)
        {

        }
        public virtual void SetDefines(List<string> LDefines, BuildType buildType)
        {
            switch (buildType)
            {
                case BuildType.ConsoleExecutable:
                    LDefines.Add("CONSOLE");
                    break;
                case BuildType.StaticLibrary:
                    LDefines.Add("LIB");
                    break;
                case BuildType.DynamicLibrary:
                    LDefines.Add("DYNL");
                    break;
            }
            if (!Config.Global.ANSI)
                LDefines.Add("_UNICODE");
            if (!Config.Global.ANSI)
                LDefines.Add("UNICODE");
            LDefines.Add(String.Format("MAIN_PROJECT_NAME=\"{0}\"", Config.Global.Project));
        }

        public virtual void SetLibraries(List<string> libs, BuildType buildType)
        {
          
        }
    }
}
