using System;
using System.Linq;
using System.Reflection;
using RemoteFork.Items;
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

        private IRemotePlugin _instance;

        public IRemotePlugin Instance =>
            _instance ?? (_instance = _assembly.CreateInstance(_type.FullName) as IRemotePlugin);

        private IPlugin _oldInstance;

        public IPlugin OldInstance =>
            _oldInstance ?? (_oldInstance = _assembly.CreateInstance(_type.FullName) as IPlugin);

        public PluginInstance(string key, Assembly assembly, Type type, PluginAttribute attribute) {
            Key = key;
            Attribute = attribute;
            _assembly = assembly;
            _type = type;
            if (ProgramSettings.Settings.CheckUpdate) {
                if (!string.IsNullOrEmpty(attribute.Github) && attribute.Github.Count(c => c == '/') == 2) {
                    UpdateController.AddUpdate(attribute.Id, new GithubProvider(attribute.Id, attribute.Github, false),
                        attribute.Version);
                }
            }
        }

        public PlayList GetPlayList(PluginContext context) {
            PlayList playList = null;

            if (typeof(IPlugin).IsAssignableFrom(_type)) {
                var pluginResponse = OldInstance.GetList(context);

                playList = new RemoteFork.Plugins.Items.Converter.PlayList(pluginResponse);
            } else if (typeof(IRemotePlugin).IsAssignableFrom(_type)) {
                playList = Instance.GetPlayList(context);
            }

            return playList;
        }

        public override string ToString() {
            return $"{Attribute.Name} {Attribute.Version} ({Attribute.Author})";
        }
    }
}
