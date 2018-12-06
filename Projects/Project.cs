using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace BearBuildTool.Projects
{
    public class ProjectListObject
    {
        public List<string> Public;
        public List<string> Private;

        public ProjectListObject()
        {
            Public = new List<string>();
            Private = new List<string>();
        }
        public void Append(ProjectListObject projectListObject)
        {
            Public.AddRange(projectListObject.Public);
        }
        public void AppendAll(ProjectListObject projectListObject)
        {
            Public.AddRange(projectListObject.Public);
            Private.AddRange(projectListObject.Private);
        }
        public void AppendInPrivate(ProjectListObject projectListObject)
        {
            Private.AddRange(projectListObject.Public);
        }
        public List<string> ToList()
        {
            List<string> list = new List<string>();
            list.AddRange(Private);
            list.AddRange(Public);
            
            return list;
        }
    }
    public abstract class Project
    {


        public ProjectListObject Include;
        public ProjectListObject IncludeInProject;
        public ProjectListObject Projects;
        public ProjectListObject Defines;
        public ProjectListObject LibrariesPath;
        public ProjectListObject LibrariesStatic;

        public List<string> Sources;
        public List<string> IncludeAutonomousProjects;
        public List<string> LibrariesDynamic;
        public string PCHFile = null;
        public string PCHIncludeFile = null;
        public bool OnlyAsStatic = false;
        public Project()
        {
            Sources = new List<string>();
            IncludeAutonomousProjects = new List<string>();
            LibrariesDynamic = new List<string>();
            Include = new ProjectListObject();
            Projects = new ProjectListObject();
            Defines = new ProjectListObject();
            LibrariesPath = new ProjectListObject();
            LibrariesStatic = new ProjectListObject();
            IncludeInProject = new ProjectListObject();
        }
        public virtual void StartBuild()
        {

        }
        public void AddSourceFiles(string path, bool andInclude = false)
        {
            Sources.AddRange(Directory.GetFiles(Path.GetFullPath(path ), "*.cpp", SearchOption.AllDirectories));
            Sources.AddRange(Directory.GetFiles(Path.GetFullPath(path), "*.c", SearchOption.AllDirectories));
            if (andInclude)
            {
                Include.Private.Add(path);
            }
        }
    }
   
       
}
