using BearBuildTool.Projects;
using BearBuildTool.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BearBuildTool.Config
{
    public enum Configure
    {
        Debug,
        Mixed,
        Release,
    }
    public enum Platform
    {
        Win32,
        Win64,
        Linux,
    }
    public class Global
    {
        public static string ProjectsPath = "projects";
        public static string IntermediatePath = "intermediate";
        public static string BinariesPath = "binaries";
        public static string BinariesPlatformPath = null;
        public static string IntermediateProjectPath = null;
        public static string Project = null;
        public static string OutProjectFileName = null;
        public static Dictionary<string,Project> ProjectsMap;
        public static Dictionary<string, string> ProjectsCSFile;
        public static Dictionary<string, Executable> ExecutableMap;
        public static string ObjectExtension;
        public static string ExecutableExtension;
        public static string StaticLibraryExtension;
        public static string DynamicLibraryExtension;
        public static string PCHExtension;
        public static Configure Configure= Configure.Debug;
        public static Platform Platform =Platform.Win32;
        public static BuildTools BuildTools;
        public static UInt64 CountBuild = 0;
        public static bool WithoutWarning = false;
        public static bool ANSI = false;

        public static bool Clean { get; internal set; }
        public static bool Rebuild { get; internal set; }

        public static bool SetPlatform(string str)
        {
            str=str.ToLower();
            if(str  ==    "win32")
            {
                ObjectExtension = ".obj";
                ExecutableExtension = ".exe";
                StaticLibraryExtension = ".lib";
                DynamicLibraryExtension = ".dll";
                PCHExtension = ".pch";
                Platform = Platform.Win32;
            }
            else if (str == "win64")
            {
                ObjectExtension = ".obj";
                ExecutableExtension = ".exe";
                StaticLibraryExtension = ".lib";
                DynamicLibraryExtension = ".dll";
                PCHExtension = ".pch";
                Platform = Platform.Win64;
            }
            else
            {
                return false;
            }
            return true;
        }
        public static bool SetConfigure(string str)
        {
            str=str.ToLower();
            if (str == "debug")
            {
                Configure = Configure.Debug;
            }
            else if (str == "mixed")
            {
                Configure = Configure.Mixed;
            }
            else if (str == "release")
            {
                Configure = Configure.Release;
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
