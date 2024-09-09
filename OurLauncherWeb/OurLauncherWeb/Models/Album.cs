using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OurLauncherWeb.Models {
    public class Album {
        public string Name { get; }
        public string Path { get; }
        public IEnumerable<Picture> Pictures { get; private set; }
        public Album(string name, string directory) {
            Name = name;
            Path = directory;
            if (Path != null && !Directory.Exists(Path)) {               
                Directory.CreateDirectory(Path);
            }
        }

        public void AddFiles(IEnumerable<Picture> files) {
            Pictures = files;
        }
    }
}
