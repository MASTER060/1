using RemoteFork.Models;
using RemoteFork.Settings;
using System.Linq;

namespace RemoteFork.Controllers.Home {
    public class PostPlugins {
        public static void PostModel(PluginsModel settings) {
            ProgramSettings.Settings.Plugins = settings.Enable;
            if (settings.EnablePlugins != null && settings.EnablePlugins.Any()) {
                ProgramSettings.Settings.EnablePlugins = settings.EnablePlugins.ToArray();
            } else {
                ProgramSettings.Settings.EnablePlugins = new string[0];
            }

            ProgramSettings.Instance.Save();
        }
    }
}
