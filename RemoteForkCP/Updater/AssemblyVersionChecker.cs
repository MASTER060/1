using System.Reflection;

namespace RemoteFork.Updater {
    public class AssemblyVersionChecker {
        public static string GetInstalledVersionNumber() {
            var version = Assembly.GetEntryAssembly()
                .GetName()
                .Version;
            string versionNumber = string.Join('.', version.Major, version.Minor, version.Build, version.Revision);
            return versionNumber;
        }
    }
}
