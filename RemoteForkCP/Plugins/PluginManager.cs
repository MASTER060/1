using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using RemoteFork.Log;
using RemoteFork.Settings;

namespace RemoteFork.Plugins {
    internal class PluginManager {
        private static readonly Logger Log = new Logger(typeof(PluginManager));

        public static readonly PluginManager Instance = new PluginManager();

        private readonly Dictionary<string, PluginInstance> _plugins = new Dictionary<string, PluginInstance>();

        private PluginManager() {
            LoadPlugins();
        }

        private void LoadPlugins() {
            string pathPlugins = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");

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
                    Log.LogError("LoadPlugins->{0}: {1}", file, exception.Message);
                }
            }
        }

        private void LoadAssembly(Assembly assembly, string hash) {
            foreach (var type in assembly.GetExportedTypes()) {
                if (typeof(IPlugin).IsAssignableFrom(type) && (type.IsAbstract == false)) {
                    var attribute = type.GetCustomAttribute<PluginAttribute>();

                    if (attribute != null) {
                        var plugin = new PluginInstance(hash, assembly, type, attribute);
                        if (!_plugins.ContainsKey(plugin.Id)) {
                            _plugins.Add(plugin.Id, plugin);

                            Log.LogDebug("Loaded plugin [id: {0}, name: {1}, type: {2}, version: {3}]", plugin.Id,
                                plugin.Name, type.FullName, plugin.Version);
                        }
                    }
                }
            }
        }

        private static string GetChecksum(string file) {
            using (var stream = File.OpenRead(file)) {
                var checksum = SHA256.Create().ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", string.Empty);
            }
        }

        public void ReimportPlugins() {
            _plugins.Clear();
            LoadPlugins();
        }

        public void RemovePlugin(string name) {
            if (_plugins.ContainsKey(name))
                _plugins.Remove(name);
        }

        public Dictionary<string, PluginInstance> GetPlugins(bool filtering = true) {
            if (filtering) {
                var dict = new Dictionary<string, PluginInstance>();

                if (ProgramSettings.Settings.Plugins) {
                    foreach (var plugin in _plugins) {
#if !DEBUG
                        if (ProgramSettings.Settings.DeveloperMode || ProgramSettings.Settings.EnablePlugins.Contains(plugin.Value.Key))
#endif
                        {
                            dict.Add(plugin.Key, plugin.Value);
                        }
                    }
                }

                return dict;
            }
            return _plugins;
        }

        public PluginInstance GetPlugin(string id) {
            if (_plugins.ContainsKey(id)) {
                if (ProgramSettings.Settings.Plugins) {
#if !DEBUG
                    if (ProgramSettings.Settings.DeveloperMode ||ProgramSettings.Settings.EnablePlugins.Contains(_plugins[id].Key))
#endif
                    {
                        return _plugins[id];
                    }
                }
            }
            return null;
        }
    }
}