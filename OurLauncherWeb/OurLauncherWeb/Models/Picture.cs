using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using MetadataExtractor;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace OurLauncherWeb.Models {
    public class Picture {
        public string Path { get; }
        public FileInfo FileInformation { get; }
        private IEnumerable<MetadataExtractor.Directory> _metaDataDirectories { get; set; }
        public string Thumbnail { get; private set; }
        public string Original { get; private set; }
        public Guid ID { get; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public double ZoomFactor { 
            get => ((Width / 200) + (Height / 200)) / 2;
        }

        public Picture(string path) {
            if (File.Exists(path)) {
                Path = path;
            } else {
                throw new FileNotFoundException($"Please check the path: {path}");
            }
            ID = Guid.NewGuid();
        }

        string ImageFormatGuidToString(ImageFormat _format) {
            if (_format.Guid == ImageFormat.Bmp.Guid) {
                return "bmp";
            } else if (_format.Guid == ImageFormat.Gif.Guid) {
                return "gif";
            } else if (_format.Guid == ImageFormat.Jpeg.Guid) {
                return "jpg";
            } else if (_format.Guid == ImageFormat.Png.Guid) {
                return "png";
            } else if (_format.Guid == ImageFormat.Icon.Guid) {
                return "ico";
            } else if (_format.Guid == ImageFormat.Emf.Guid) {
                return "emf";
            } else if (_format.Guid == ImageFormat.Exif.Guid) {
                return "exif";
            } else if (_format.Guid == ImageFormat.Tiff.Guid) {
                return "tiff";
            } else if (_format.Guid == ImageFormat.Wmf.Guid) {
                return "wmf";
            } else {
                return null;
            }
        }

        public string GetThumnbnail() {
            if (Thumbnail == null) {
                Image image = Image.FromFile(Path);
                Image.GetThumbnailImageAbort myCallback = new Image.GetThumbnailImageAbort(() => { return true; });
                Image thumbnail = image.GetThumbnailImage(64, 64, myCallback, IntPtr.Zero);
                string format = ImageFormatGuidToString(thumbnail.RawFormat);
                using (MemoryStream imageStream = new MemoryStream()) {
                    if (format == null) {
                        thumbnail.Save(imageStream, ImageFormat.Jpeg);
                    } else {
                        thumbnail.Save(imageStream, thumbnail.RawFormat);
                    }
                    Thumbnail = String.Format("data:image/{0};base64,{1}", format ?? "jpg", Convert.ToBase64String(imageStream.ToArray()));
                }
                thumbnail.Dispose();
                image.Dispose();
            }
            return Thumbnail;
        }

        public string GetOriginal() {
            if (Original == null) {

                Image image = Image.FromFile(Path);
                string format = ImageFormatGuidToString(image.RawFormat);
                using (MemoryStream imageStream = new MemoryStream()) {
                    if (format == null) {
                        image.Save(imageStream, ImageFormat.Jpeg);
                    } else {
                        image.Save(imageStream, image.RawFormat);
                    }
                    Original = String.Format("data:image/{0};base64,{1}", format ?? "jpg", Convert.ToBase64String(imageStream.ToArray()));
                }
                Width = image.Width;
                Height = image.Width;
                image.Dispose();
            }
            return Original;
        }

        public string GetPictureBySize(int width, int height) {
            string base64 = "";
            Image image = Image.FromFile(Path);
            Bitmap TmpImg = new Bitmap(image, width, height);
            string format = ImageFormatGuidToString(TmpImg.RawFormat);
            using (MemoryStream imageStream = new MemoryStream()) {
                if (format == null) {
                    TmpImg.Save(imageStream, ImageFormat.Jpeg);
                } else {
                    TmpImg.Save(imageStream, image.RawFormat);
                }
                base64 = String.Format("data:image/{0};base64,{1}", format ?? "jpg", Convert.ToBase64String(imageStream.ToArray()));
            }
            TmpImg.Dispose();
            image.Dispose();
            return base64;
        }

        public string GetMetaData() {
            _metaDataDirectories = ImageMetadataReader.ReadMetadata(Path);
            StringBuilder metaData = new StringBuilder("");
            foreach (var directory in _metaDataDirectories)
                foreach (var tag in directory.Tags)
                    metaData.AppendLine($"{directory.Name} - {tag.Name} = {tag.Description}");
            return metaData.ToString();
        }

        public bool MovePicture(string newPath, string newFile = null) {
            try {
                string finalPath = newPath;
                FileInfo fileInfo = new FileInfo(Path);
                if(!System.IO.Directory.Exists(finalPath)) {
                    finalPath = System.IO.Path.Combine(Path.Replace(fileInfo.Name, ""), newPath);
                    if (!System.IO.Directory.Exists(finalPath)) {
                        System.IO.Directory.CreateDirectory(finalPath);
                    }
                }

                if(Startup.Config["PictureViewer:copy"] == "true") {
                    File.Copy(Path, System.IO.Path.Combine(finalPath, newFile ?? fileInfo.Name));
                } else {
                    File.Move(Path, System.IO.Path.Combine(finalPath, newFile ?? fileInfo.Name));
                }            
            } catch (Exception ex) {
                return false;
            }
            return true;
        }
    }
}
