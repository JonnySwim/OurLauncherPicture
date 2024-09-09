using ElectronNET.API;
using Microsoft.AspNetCore.Mvc;
using OurLauncherWeb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OurLauncherWeb.Controllers {
    public class HomeController : Controller {

        public ActionResult Index() {          
            return View();
        }

        public ActionResult PictureViewer(string path) {
            ViewBag.Files = GetFiles(path);
            ViewBag.Folder = "";
            return PartialView();
        }

        public IEnumerable<Picture> GetFiles(string path) {
            Album folder;
            if (!string.IsNullOrWhiteSpace(path)) {
                folder = PictureCache.GetFolder(path) ?? new Album(null, path);
                if (folder.Pictures == null || !folder.Pictures.Any()) {
                    PictureLoader pictureLoader = new PictureLoader(path);
                    List<Task> tasks = new List<Task>();
                    folder.AddFiles(pictureLoader.GetPictures());
                }
            } else {
                folder = PictureCache.GetLastFolder() ?? null;
            }

            return folder?.Pictures;
        }

        public string Thumbnail(string path, string key) {
            Picture picture = PictureCache.GetPicture(path.Replace("|||",@"\"));
            string thumbnail = picture.GetThumnbnail();
            return thumbnail;
        }

        [HttpPost]
        public JsonResult GetOriginal(string path) {
            Picture picture = PictureCache.GetPicture(path.Replace("|||", @"\"));
            return Json(new { base64 = picture.GetOriginal(), zoomFactor = picture.ZoomFactor });
        }

        [HttpPost]
        public string GetNext(string currentfolder, string currentpicture) {
            Album folder = PictureCache.GetFolder(currentfolder.Replace("|||", @"\"));
            IEnumerable<Picture> pictures = folder.Pictures;
            Picture picture = folder.Pictures?.Where(x => x.Path == currentpicture.Replace("|||", @"\")).FirstOrDefault();
            int nextIndex = pictures.ToList().IndexOf(picture) + 1;
            return pictures.Count() > nextIndex ? pictures.ElementAt(nextIndex).Path : null;
        }

        [HttpPost]
        public string GetPrevious(string currentfolder, string currentpicture) {
            Album folder = PictureCache.GetFolder(currentfolder.Replace("|||", @"\"));
            IEnumerable<Picture> pictures = folder.Pictures;
            Picture picture = folder.Pictures?.Where(x => x.Path == currentpicture.Replace("|||", @"\")).FirstOrDefault();
            int nextIndex = pictures.ToList().IndexOf(picture) - 1;
            return pictures.Count() > nextIndex ? pictures.ElementAt(nextIndex).Path : null;
        }

        [HttpPost]
        public JsonResult MovePicture(string path, string newPath) {
            Picture picture = PictureCache.GetPicture(path.Replace("|||", @"\"));
            return Json(picture.MovePicture(newPath.Replace("|||", @"\")));
        }

        [HttpPost]
        public JsonResult NewAlbum(string name) {
            string path = ElecronWindow.OpenFolder(@"" + Startup.Config["PictureViewer:defaultCreatePath"] + @"\" + name);
            Album album = new Album(name, path);
            return Json(album);
        }

        public void CloseWindow() {
            ElecronWindow.CloseWindow();
        }
        public void ReloadWindow() {
            ElecronWindow.ReloadWindow();
        }

        public IActionResult OpenFolder() {
            string path = ElecronWindow.OpenFolder();
            ViewBag.Files = GetFiles(path);
            ViewBag.Folder = path;
            return PartialView("Thumbnails");
        }

        public void OpenDevTools() {
            ElecronWindow.OpenDevTools();
        }

        [HttpPost]
        public async Task<JsonResult> GetWindowSize() {
            return Json(await ElecronWindow.GetWindowSize());
        }
    }
}
