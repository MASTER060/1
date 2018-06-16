using System.Runtime.InteropServices;

namespace RemoteFork.Updater {
    public static class OSVersion {
        public static string GetOSVersion() {
            string osVersion = GetOSPlatform();
            switch (RuntimeInformation.OSArchitecture) {
                case Architecture.Arm:
                    osVersion += "-arm";
                    break;
                case Architecture.Arm64:
                    osVersion += "-arm64";
                    break;
                case Architecture.X64:
                    osVersion += "-x64";
                    break;
                case Architecture.X86:
                    osVersion += "-x86";
                    break;
            }

            return osVersion;
        }

        private static string GetOSPlatform() {
            string osPlatform = string.Empty;
            // Check if it's windows 
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            osPlatform = isWindows ? "win" : osPlatform;
            // Check if it's osx 
            bool isOSX = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            osPlatform = isOSX ? "osx" : osPlatform;
            // Check if it's Linux 
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            osPlatform = isLinux ? "linux" : osPlatform;
            return osPlatform;
        }
    }
}
