using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteFork.Updater {
    public static class UpdateController {
        private static readonly Dictionary<string, Updater> _updaters = new Dictionary<string, Updater>();

        public static void AddUpdate(string id, GithubProvider provider, string installedVersion) {
            var updater = new Updater(provider, installedVersion);
            if (_updaters.ContainsKey(id)) {
                _updaters[id] = updater;
            } else {
                _updaters.Add(id, updater);
            }
        }

        public static async Task CheckUpdates() {
            foreach (var updater in _updaters.Where(i => !i.Value.FindUpdate)) {
                await updater.Value.CheckUpdate();
            }
        }

        public static bool IsUpdateAvaiable(string id) {
            return _updaters[id].FindUpdate;
        }

        public static Updater GetUpdater(string id) {
            return _updaters[id];
        }
    }
}
