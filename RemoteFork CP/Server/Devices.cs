using System.Collections.Generic;

namespace RemoteFork.Server {
    public static class Devices {
        private static readonly HashSet<string> devices = new HashSet<string>();

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
