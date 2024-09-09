using Microsoft.AspNetCore.Mvc;
using OurLauncherWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OurLauncherWeb.ViewComponents.PictureComponent {
    public class Thumbnail : ViewComponent {
        public Thumbnail() { }
        public async Task<IViewComponentResult> InvokeAsync(KeyValuePair<string, string> fileInfo) {
            Picture picture = PictureCache.GetPicture(fileInfo.Value);
            string thumbnail = picture.GetThumnbnail();
            return View(new KeyValuePair<string, string>(fileInfo.Key, thumbnail));
        }
    }
}
