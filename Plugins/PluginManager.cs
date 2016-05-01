using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using RemoteFork.Plugin;

namespace RemoteFork.Plugins {
    internal static class PluginManager {
        private static readonly Dictionary<string, BasePlugin> Plugins = new Dictionary<string, BasePlugin>();

        static PluginManager() {
            LoadPlugins();
        }

        private static void LoadPlugins() {
            string pathPlugins = Path.Combine(Environment.CurrentDirectory, "Plugins");
            if (Directory.Exists(pathPlugins)) {
                var files = Directory.GetFiles(pathPlugins, "*.dll");
                foreach (string dllFile in files) {
                    try {
                        var aboutAssembly = Assembly.LoadFrom(dllFile);
                        foreach (var elem in aboutAssembly.GetExportedTypes()) {
                            var plugin = (BasePlugin) Activator.CreateInstance(elem);
                            if (!Plugins.ContainsKey(plugin.Name)) {
                                Plugins.Add(plugin.Name, plugin);
                            }
                        }
                    } catch (Exception ex) {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        public static void ReimportPlugins() {
            Plugins.Clear();
            LoadPlugins();
        }

        public static void RemovePlugin(string name) {
            if (Plugins.ContainsKey(name)) {
                Plugins.Remove(name);
            }
        }

        public static Dictionary<string, BasePlugin> GetPlugins() {
            return Plugins;
        }

        public static BasePlugin GetPlugin(string name) {
            return Plugins.ContainsKey(name) ? Plugins[name] : null;
        }
    }
}
