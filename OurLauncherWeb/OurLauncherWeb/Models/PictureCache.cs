using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OurLauncherWeb.Models {
    public static class PictureCache {
        private static object _access = new object();
        private static ConcurrentDictionary<string, Picture> _pictures = new ConcurrentDictionary<string, Picture>();
        private static BlockingCollection<Album> _folders = new();

        public static Picture GetPicture(string path) {
            lock (_access) {
                if(!_pictures.TryGetValue(path, out Picture picture)) {
                    picture = new Picture(path);
                    _pictures.TryAdd(path, picture);
                }
                return picture;
            }
        }

        public static Album GetFolder(string path) {
            lock (_access) {
                if (!_folders.Any(x => x.Path == path)) {
                    Album album = new Album(null, path);
                    _folders.TryAdd(album);
                }
                return _folders.Where(x => x.Path == path).FirstOrDefault();
            }
        }

        public static Album GetLastFolder() {
            return _folders.FirstOrDefault();
        }
    }
}
