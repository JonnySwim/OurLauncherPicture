using ElectronNET.API;
using ElectronNET.API.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OurLauncherWeb.Models {
    public static class ElecronWindow {
         
        public static BrowserWindow Window { get; set; }

        public static async void OpenWindow() {

            Window = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions {
                TitleBarStyle = TitleBarStyle.hidden,
                Frame = false,
                Width = 800,
                Height = 600,
                Transparent = true,
                Movable = true,
                
            });

            Window.OnReadyToShow += () => Window.Show();
        }

        public static void CloseWindow() {
            if (HybridSupport.IsElectronActive && Window != null) {
                Window.Close();
            }
        }

        public static void ReloadWindow() {
            if (HybridSupport.IsElectronActive && Window != null) {
                Window.Reload();
            }
        }

        public static string OpenFolder(string defaultPath = null) {
            if (HybridSupport.IsElectronActive) {
                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                OpenDialogOptions options = new();
                OpenDialogProperty[] properties = new OpenDialogProperty[3];
                properties[0] = OpenDialogProperty.openDirectory;
                properties[1] = OpenDialogProperty.createDirectory;
                properties[2] = OpenDialogProperty.promptToCreate;
                options.DefaultPath = defaultPath ?? Startup.Config["PictureViewer:defaultCreatePath"];
                options.Properties = properties;
                Task<string[]> task = Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);
                task.Wait();
                return task.Result[0];
            } else {
                return Startup.Config["PictureViewer:defaultPath"];
            }
        }

        public static void OpenDevTools() {
            Electron.WindowManager.BrowserWindows.First()?.WebContents.OpenDevTools();
        }

        public static async Task<int[]> GetWindowSize() {
            if (HybridSupport.IsElectronActive) {
                return await Window.GetSizeAsync();
            } else {
                return new int[] { 500, 500 };
            }
        }
    }
}
