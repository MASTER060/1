using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RemoteFork.Models;
using RemoteFork.Plugins;
using RemoteFork.Settings;

namespace RemoteFork.Controllers.Home {
    public static class GetPlugins {
        public static PluginsModel GetModel(dynamic viewBag) {
            var pluginsList = new List<SelectListItem>();
            var allPlugins = PluginManager.Instance.GetPlugins(false);
            foreach (var plugin in allPlugins) {
                pluginsList.Add(new SelectListItem() {
                    Text = plugin.Value.ToString(),
                    Value = plugin.Value.Key
                });
            }

            var model = new PluginsModel() {
                Enable = ProgramSettings.Settings.Plugins,
                IconsEnable = ProgramSettings.Settings.PluginIcons,
                Plugins = pluginsList,
                EnablePlugins = ProgramSettings.Settings.EnablePlugins
            };
            return model;
        }
    }
}
