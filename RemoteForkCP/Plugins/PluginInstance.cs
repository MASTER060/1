using System;
using System.Linq;
using System.Reflection;
using RemoteFork.Settings;
using RemoteFork.Updater;

namespace RemoteFork.Plugins {
    internal class PluginInstance {
        public readonly string Key;
        public readonly PluginAttribute Attribute;

        public string Id => Attribute.Id;
        public string Name => Attribute.Name;

        private readonly Assembly _assembly;
        private readonly Type _type;

        private IPlugin _instance;

        public PluginInstance(string key, Assembly assembly, Type type, PluginAttribute attribute) {
            Key = key;
            Attribute = attribute;
            _assembly = assembly;
            _type = type;
            if (ProgramSettings.Settings.CheckUpdate) {
                if (!string.IsNullOrEmpty(attribute.Github) && attribute.Github.Count(c => c == '/') == 2) {
                    UpdateController.AddUpdate(key, new GithubProvider(attribute.Id, attribute.Github, false),
                        attribute.Version);
                }
            }
        }

        public IPlugin Instance => _instance ?? (_instance = _assembly.CreateInstance(_type.FullName) as IPlugin);

        public override string ToString() {
            return $"{Attribute.Name} {Attribute.Version} ({Attribute.Author})";
        }
    }
}
