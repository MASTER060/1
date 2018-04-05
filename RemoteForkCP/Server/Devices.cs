using System.Collections.Generic;
using System.Linq;

namespace RemoteFork.Server {
    public static class Devices {
        private static readonly HashSet<string> devices = new HashSet<string>();

        public static List<string> Get() {
            return devices.ToList();
        }
        public static void Add(string device) {
            if (!Contains(device)) {
                devices.Add(device);
            }
        }

        public static void Remove(string device) {
            if (Contains(device)) {
                devices.Remove(device);
            }
        }

        public static bool Contains(string device) {
            return devices.Contains(device);
        }

        public static void Clear() {
            devices.Clear();
        }
    }
}
