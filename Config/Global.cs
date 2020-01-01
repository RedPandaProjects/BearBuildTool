using BearBuildTool.Projects;
using BearBuildTool.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BearBuildTool.Config
{
    public enum Configure
    {
        None,
        Debug,
        Mixed,
        Release,
    }
    public enum Platform
    {
        None,
        Win32,
        Win64,
        MinGW,
        Linux,
    }
    public class Global
    {
        public static bool DevVersion = true;
        public static string BasePath = null;
        public static string ProjectsPath = "projects";
        public static string IntermediatePath = "intermediate";
        public static string BinariesPath = "binaries";
        public static string BinariesPlatformPath = null;
        public static string IntermediateProjectPath = null;
        public static string Project = null;
        public static string OutProjectFileName = null;
        public static Dictionary<Platform, Dictionary<Configure, Dictionary<string, Project>>> ProjectsMap;
        public static bool vs2019 = true;

        public static Dictionary<string, string> ProjectsCSFile;
        public static Dictionary<Platform, Dictionary<Configure, Dictionary<string, Executable>>> ExecutableMap;
        public static string ObjectExtension;
        public static string ExecutableExtension;
        public static string StaticLibraryExtension;
        public static string DynamicLibraryExtension;
        public static string PCHExtension;
        public static Configure Configure = Configure.None;
        public static Platform Platform = Platform.None;
        public static BuildTools BuildTools;
        public static UInt64 CountBuild = 0;
        public static bool WithoutWarning = false;
        public static bool UNICODE = true;

        public static int CountThreads = 32;

        public static bool Clean = false;
        public static bool Rebuild = false;
        public static string MinGWPath = "C:/MinGW";

        public static bool SetPlatform(string str)
        {
            str=str.ToLower();
            if (str == "mingw")
            {
                ObjectExtension = ".o";
                ExecutableExtension = ".exe";
                StaticLibraryExtension = ".a";
                DynamicLibraryExtension = ".dll";
                PCHExtension = ".gch";
                Platform = Platform.MinGW ;
            }
            else if(str  ==    "win32")
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
            else if(str=="linux")
            {
                ObjectExtension = ".o";
                ExecutableExtension = "";
                StaticLibraryExtension = "";
                DynamicLibraryExtension = "";
                PCHExtension = ".gch";
                Platform = Platform.Linux;
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
        public static string Windows10SDK = String.Empty;
        public static bool Windows10SDKUsing = true;
        private static int VersionConfig = 4;
        public static bool IsWindows=true;
        

        public static void SaveConfig()
        {
            try
            { File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.bin")); }
            catch { }
            try
            {
                BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.bin"), FileMode.Create));
                {
                    writer.Write(VersionConfig);
                    writer.Write(Windows10SDK);
                    writer.Write(Windows10SDKUsing);
                    writer.Write(MinGWPath);
                    writer.Write(BasePath);
                    writer.Write(DevVersion);
                }
                writer.Close();
            }
            catch { }
        }
        public static void LoadConfig()
        {
            try
            {
                BinaryReader reader = new BinaryReader(File.Open(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.bin"), FileMode.Open,FileAccess.Read));
                try
                {
                    int version = reader.ReadInt32();
                    if (version == 1)
                    {
                        Windows10SDK = reader.ReadString();
                        Windows10SDKUsing = reader.ReadBoolean();
                    }
                    else if (version == 2)
                    {
                        Windows10SDK = reader.ReadString();
                        Windows10SDKUsing = reader.ReadBoolean();
                        MinGWPath = reader.ReadString();
                    }
                    else if (version == 3)
                    {
                        Windows10SDK = reader.ReadString();
                        Windows10SDKUsing = reader.ReadBoolean();
                        MinGWPath = reader.ReadString();
                        BasePath = reader.ReadString();
                    }
                    else  if (version == VersionConfig)
                    {
                        Windows10SDK = reader.ReadString();
                        Windows10SDKUsing = reader.ReadBoolean();
                        MinGWPath = reader.ReadString();
                        BasePath = reader.ReadString();
                        DevVersion = reader.ReadBoolean();
                    }
                }
                catch { }
                reader.Close();
            }
            catch { }
        }
    }
}
