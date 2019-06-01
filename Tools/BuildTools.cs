using BearBuildTool.Projects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace BearBuildTool.Tools
{
    public class BuildTools
    {
        public virtual BuildTools Create()
        {
            return new BuildTools();
        }
        protected List<string> BuildObjects_LInclude = null;
        protected List<string> BuildObjects_LDefines = null;
        protected string BuildObjects_pch = null;
        protected string BuildObjects_pchH = null;
        protected string BuildObjects_objs_out = null;
        protected BuildType BuildObjects_buildType = BuildType.StaticLibrary;
        protected string BuildObjects_NameProject;
        public virtual void BuildObjectsStart(string NameProject,List<string> LInclude, List<string> LDefines, string pch, string pchH, string objs_out, BuildType buildType)
        {
            BuildObjects_LInclude = LInclude;
            BuildObjects_LDefines = LDefines;
            BuildObjects_pch = pch;
            BuildObjects_pchH = pchH;
            BuildObjects_objs_out = objs_out;
            BuildObjects_buildType = buildType;
            BuildObjects_NameProject = NameProject;
        }
        public virtual void BuildObjectsEnd()
        {
            BuildObjects_LInclude = null;
            BuildObjects_LDefines = null;
            BuildObjects_pch = null;
            BuildObjects_pchH = null;
            BuildObjects_objs_out = null;
            BuildObjects_buildType = BuildType.StaticLibrary;
            BuildObjects_NameProject = String.Empty;
        }
        public virtual void BuildObjectPush(string source)
        {
            Console.WriteLine(String.Format("Сборка {0}", Path.GetFileName(source)));
            BuildObject(BuildObjects_NameProject, BuildObjects_LInclude, BuildObjects_LDefines, BuildObjects_pch, BuildObjects_pchH, false, source, Path.Combine(BuildObjects_objs_out, Path.GetFileNameWithoutExtension(source) + Config.Global.ObjectExtension), BuildObjects_buildType);
        }

        public virtual void BuildObject(string NameProject, List<string> LInclude, List<string> LDefines, string pch,string pchH,bool createPCH, string source, string obj,BuildType buildType)
        {

        }
        public virtual void BuildDynamicLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outDynamicLib, string outStaticLib)
        {

        }
        public virtual void BuildResource(List<string> LInclude, List<string> LDefines, string source, string obj, BuildType buildType)
        {

        }
        public virtual void BuildExecutable(List<string> objs, List<string> libs, List<string> libsPath, string Executable, string outStaticLib, bool Console)
        {

        }
        public virtual void BuildStaticLibrary(List<string> objs, List<string> libs, List<string> libsPath, string outStaticLib)
        {

        }
        public virtual void SetDefines(List<string> LDefines,string ProjectOutName, BuildType buildType)
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
            if (Config.Global.UNICODE)
                LDefines.Add("_UNICODE");
            if (Config.Global.UNICODE)
                LDefines.Add("UNICODE");
            LDefines.Add(String.Format("MAIN_PROJECT_NAME=\"{0}\"", Config.Global.Project));
            LDefines.Add(String.Format("PROJECT_OUT=\"{0}\"",Path.GetFileName( ProjectOutName)));
        }

        public virtual void SetLibraries(List<string> libs, BuildType buildType)
        {
          
        }
    }
}
