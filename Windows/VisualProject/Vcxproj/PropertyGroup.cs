using BearBuildTool.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BearBuildTool.Windows.VisualProject.Vcxproj
{

    class PropertyGroup : XmlObject
    {
        public enum ELabel
        {
            EL_Default,
            EL_Globals,
            EL_Configuration,
            EL_UserMacros,
        }
        public ELabel Label = ELabel.EL_Default;
        public class SGlobals
        {
            public SGlobals()
            {
                ProjectGuid = Guid.NewGuid();
                Keyword = "Win32Proj";
                RootNamespace = String.Empty;
            }
            public Guid ProjectGuid;
            public string Keyword;
            public string RootNamespace;
        };
        public SGlobals Globals = new SGlobals();
        /*Makefile*/
        public class SConfiguration
        {
            public string Configuration = "";
            public string Platform = "";
            public string PlatformToolset = "v141";
        }
        public SConfiguration Configuration = new SConfiguration();
        public class SDefault
        {
            public string Configuration = "";
            public string Platform = "";
            public string NMakeOutput = "";
            public string NMakePreprocessorDefinitions = "";
            public string NMakeIncludeSearchPath = "";
            public string OutDir = "";
            public string IntDir = "";
            public string NMakeBuildCommandLine = "";
            public string NMakeReBuildCommandLine = "";
            public string NMakeCleanCommandLine = "";
        };
        public SDefault Default = new SDefault();
        public override string GetName()
        {
            return "PropertyGroup";
        }
        public override XmlObject Create()
        {
            return new PropertyGroup();
        }
        override protected bool LoadImpl(ref XmlNode parent)
        {
           /* if (HasAttribute(ref parent, "Label"))
            {
                string Label = GetAttribute(ref parent, "Label").Value;
                if(Label=="Globals")
                {
                    if (!HasNode(ref parent, "ProjectGuid"))
                        return false;
                    if (Guid.TryParseExact(GetNode(ref parent, "ProjectGuid").InnerText, "B", out Globals.ProjectGuid))
                        return false;

                    if (!HasNode(ref parent, "Keyword"))
                        return false;
                    Globals.Keyword = GetNode(ref parent, "Keyword").InnerText;

                    if (!HasNode(ref parent, "RootNamespace"))
                        return false;
                    Globals.RootNamespace = GetNode(ref parent, "RootNamespace").InnerText;
                }
                else if( Label== "Configuration")
                {
                    Scanner scanner = new Scanner();
                    if (!HasAttribute(ref parent, "Condition"))
                        return false;
                    object[] targets = new object[4];
                    targets[0] = "";
                    targets[1] = "";
                    scanner.Scan(GetAttribute(ref parent, "Condition").Value, "'$(Configuration)|$(Platform)'=='{String}|{String}'", targets);
                    Configuration.Configuration = targets[0].ToString();
                    Configuration.Platform = targets[1].ToString();
                    if (!HasNode(ref parent, "Makefile"))
                        return false;

                }
                else
                {
                    return false;
                }
            }
            else
            {

            }*/
            return false;
        }
        override protected void SaveImpl(ref XmlDocument xmlDocument, ref XmlNode parent)
        {
            switch (Label)
            {
                case ELabel.EL_Default:
                    {
                        AppendAttribute(ref xmlDocument, ref parent, "Condition", String.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", Default.Configuration, Default.Platform));
                        AppendNode(ref xmlDocument, ref parent, "NMakeOutput", Default.NMakeOutput);
                        AppendNode(ref xmlDocument, ref parent, "NMakePreprocessorDefinitions", Default.NMakePreprocessorDefinitions);
                        AppendNode(ref xmlDocument, ref parent, "NMakeIncludeSearchPath", Default.NMakeIncludeSearchPath);
                        AppendNode(ref xmlDocument, ref parent, "OutDir", Default.OutDir);
                        AppendNode(ref xmlDocument, ref parent, "IntDir", Default.IntDir);
                        AppendNode(ref xmlDocument, ref parent, "NMakeBuildCommandLine", Default.NMakeBuildCommandLine);
                        AppendNode(ref xmlDocument, ref parent, "NMakeReBuildCommandLine", Default.NMakeReBuildCommandLine);
                        AppendNode(ref xmlDocument, ref parent, "NMakeCleanCommandLine", Default.NMakeCleanCommandLine);
                    }
                    break;
                case ELabel.EL_Globals:
                    {
                        AppendAttribute(ref xmlDocument, ref parent, "Label", "Globals");
                        AppendNode(ref xmlDocument, ref parent, "ProjectGuid", Globals.ProjectGuid.ToString("B"));
                        AppendNode(ref xmlDocument, ref parent, "Keyword", Globals.Keyword);
                        AppendNode(ref xmlDocument, ref parent, "RootNamespace", Globals.RootNamespace);
                    }
                    break;
                case ELabel.EL_Configuration:
                    {
                        AppendAttribute(ref xmlDocument, ref parent, "Label", "Configuration");
                        AppendAttribute(ref xmlDocument, ref parent, "Condition", String.Format("'$(Configuration)|$(Platform)'=='{0}|{1}'", Configuration.Configuration, Configuration.Platform));
                        AppendNode(ref xmlDocument, ref parent, "ConfigurationType", "Makefile");
                        AppendNode(ref xmlDocument, ref parent, "PlatformToolset", Configuration.PlatformToolset );
                        AppendNode(ref xmlDocument, ref parent, "UseDebugLibraries", Configuration.Configuration=="Release"? "false" : "true");
                    }
                    break;
                case ELabel.EL_UserMacros:
                    {
                        AppendAttribute(ref xmlDocument, ref parent, "Label", "UserMacros");
                    }
                    break;
                default:
                    return;
            }

        }
    }
}
