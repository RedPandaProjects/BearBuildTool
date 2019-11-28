using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BearBuildTool.VisualCode
{
    class VCWorkspace
    {

        public class Folder
        {
            public Folder(string Path) { path = Path; }
            public string path;
        }
        public IList<Folder> folders = new List<Folder>();
        public class Settings
        {

            public class Files_associations
            {
                public string iostream = "cpp";
            }
            [JsonProperty("files.associations")]
            public Files_associations files_associations = new Files_associations();
        }
        public Settings settings = new Settings();
    }
}
