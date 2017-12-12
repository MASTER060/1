using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace RemoteFork.Plugins {
    internal class PluginManager {
        private static readonly Microsoft.Extensions.Logging.ILogger Log = Program.LoggerFactory.CreateLogger<PluginManager>();

        public static readonly PluginManager Instance = new PluginManager();

        private readonly Dictionary<string, PluginInstance> plugins = new Dictionary<string, PluginInstance>();

        private PluginManager() {
            LoadPlugins();
        }

        private void LoadPlugins() {
#if DEBUG
            string pathPlugins = Path.Combine(Environment.CurrentDirectory, "bin\\Debug\\netcoreapp2.0", "Plugins");
#else
            string pathPlugins = Path.Combine(Environment.CurrentDirectory, "Plugins");
#endif

            if (Directory.Exists(pathPlugins)) {
                LoadAssemblies(pathPlugins);
            }
        }

        private void LoadAssemblies(string pathPlugins) {
            var dir = new DirectoryInfo(pathPlugins);

            foreach (var file in dir.GetFiles("*.dll")) {
                try {
                    LoadAssembly(Assembly.LoadFrom(file.FullName), GetChecksum(file.FullName));
                } catch (Exception exception) {
                    Log.LogError(exception, "LoadPlugins->{0}: {1}", file, exception.Message);
                }
            }
        }

        private void LoadAssembly(Assembly assembly, string hash) {
            foreach (var type in assembly.GetExportedTypes()) {
                if (typeof(IPlugin).IsAssignableFrom(type) && (type.IsAbstract == false)) {
                    var attributes = type.GetCustomAttributes(true);

                    if (attributes.Length > 0) {
                        var attribute =
                            (PluginAttribute) attributes.FirstOrDefault(i => i.GetType() == typeof(PluginAttribute));
                        if (attribute != null) {
                            var plugin = new PluginInstance(hash, assembly, type, attribute);
                            if (!plugins.ContainsKey(plugin.Id)) {
                                plugins.Add(plugin.Id, plugin);

                                Log.LogDebug("Loaded plugin [id: {0}, name: {1}, type: {2}, version: {3}]", plugin.Id,
                                    plugin.Name, type.FullName, plugin.Version);
                            }
                        }
                    }
                }
            }
        }

        private static string GetChecksum(string file) {
            using (var stream = File.OpenRead(file)) {
                var sha = new SHA256Managed();
                var checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", string.Empty);
            }
        }

        public void ReimportPlugins() {
            plugins.Clear();
            LoadPlugins();
        }

        public void RemovePlugin(string name) {
            if (plugins.ContainsKey(name))
                plugins.Remove(name);
        }

        public Dictionary<string, PluginInstance> GetPlugins(bool filtering = true) {
            if (filtering) {
                var dict = new Dictionary<string, PluginInstance>();

                if (SettingsManager.Settings.Plugins && (SettingsManager.Settings.EnablePlugins != null)) {
#if DEBUG
                    foreach (var plugin in plugins)
                    {
                        dict.Add(plugin.Key, plugin.Value);
                    }
#else
                    foreach (var plugin in plugins.Where(
                        plugin => SettingsManager.Settings.EnablePlugins.Contains(plugin.Value.Key))) {
                        dict.Add(plugin.Key, plugin.Value);
                    }
#endif
                }

                return dict;
            }
            return plugins;
        }

        public PluginInstance GetPlugin(string id) {
            if (plugins.ContainsKey(id)) {
                if (SettingsManager.Settings.Plugins && (SettingsManager.Settings.EnablePlugins != null)) {
#if DEBUG
                    return plugins[id];
#else
                    if (SettingsManager.Settings.EnablePlugins.Contains(plugins[id].Key)) {
                        return plugins[id];
                    }
#endif
                }
            }
            return null;
        }
    }
}
