using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OurLauncherWeb.Models {
    public class Launcher {

        public string Name { get; }
        public List<LauncherItem> LauncherItems { get; private set; }
        private readonly object Access;
        public Launcher(string name) {
            Access = new object();
            Name = name;
            LauncherItems = new List<LauncherItem>();
        }

        public void AddFolder(string path) {
            try {
                DirectoryInfo directory = new DirectoryInfo(path);
                if (directory != null) {
                    foreach (FileInfo file in directory.GetFiles("*", SearchOption.AllDirectories)) {
                        if (file != null && file.Extension == ".exe" || file.Extension == ".lnk") {
                            Icon iconForFile = Icon.ExtractAssociatedIcon(file.FullName);
                            lock (Access) {
                                LauncherItem launcherItem = new LauncherItem(file.Name, file.FullName, iconForFile);
                                if(!LauncherItems.Contains(launcherItem)) {
                                    LauncherItems.Add(launcherItem);
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }


        public class LauncherItem { 
            public string Name { get; }
            public string Path { get; }
            public string FileIcon { get; }

            public LauncherItem(string name, string path, Icon icon) {
                Name = name;
                Path = path;
                Bitmap bitmapIcon = icon.ToBitmap();
                MemoryStream imageStream = new MemoryStream();
                bitmapIcon.Save(imageStream, ImageFormat.Jpeg);
                string base64 = Convert.ToBase64String(imageStream.ToArray());
                FileIcon = String.Format("data:image/jpeg;base64,{0}", base64);
            }

            public void Launch() {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    CreateNoWindow = false,
                    UseShellExecute = true,
                    FileName = Path,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                try {
                    Process exeProcess = Process.Start(startInfo);
                    exeProcess.Dispose();
                } catch (Exception ex){
                    Debug.WriteLine(ex);
                }
            }
        }
    }
}
