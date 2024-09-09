using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OurLauncherWeb.Models {
    public static class LauncherCache {
        public static List<Launcher> Launchers;

        static LauncherCache() {
            Launchers = new List<Launcher>();
        }
        public static void AddLauncher(Launcher launcher) {
            Launchers.Add(launcher);
        }
    }
}
