using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OurLauncherWeb.Models {

    public class PictureLoader {
        private string Path { get; }
        private static readonly string[] _imageExtensions = { ".JPG", ".JPEG", ".BMP", ".GIF", ".PNG" };
        public List<Picture> Pictures { get; private set; }

        public PictureLoader(string path) {
            Path = path;
        }

        public IEnumerable<Picture> GetPictures() {
            Pictures = new();
            DirectoryInfo directory = new DirectoryInfo(Path);
            if (directory != null) {
                List<FileInfo> files = directory.GetFiles("*", SearchOption.AllDirectories).Where(x => _imageExtensions.Contains(x.Extension.ToUpperInvariant())).ToList();
                if(files.Count() > 0) {
                    foreach(FileInfo file in files) {
                        Pictures.Add(PictureCache.GetPicture(file.FullName));
                    }
                }
            }
            return Pictures;
        }

        public Task<Picture> GetPictureAsync(string path) {
            return (Task<Picture>)Task.Run(() => {
                return new Picture(path);
            } );
        }
    }
}
