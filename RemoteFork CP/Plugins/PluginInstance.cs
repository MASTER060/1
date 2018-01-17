using System;
using System.Reflection;

namespace RemoteFork.Plugins {
    internal class PluginInstance {
        public readonly string Author;
        public readonly string Description;
        public readonly string Id;
        public readonly string ImageLink;
        public readonly string Key;
        public readonly string Name;
        public readonly string Version;
        private readonly Assembly _assembly;
        private readonly Type _type;

        private IPlugin _instance;

        public PluginInstance(string key, Assembly assembly, Type type, PluginAttribute attribute) {
            Key = key;
            Id = attribute.Id;
            Name = attribute.Name;
            Description = attribute.Description;
            ImageLink = attribute.ImageLink;
            Version = attribute.Version;
            Author = attribute.Author;
            _assembly = assembly;
            _type = type;
        }

        public IPlugin Instance => _instance ?? (_instance = _assembly.CreateInstance(_type.FullName) as IPlugin);

        public override string ToString() {
            return $"{Name} {Version} ({Author})";
        }
    }
}
