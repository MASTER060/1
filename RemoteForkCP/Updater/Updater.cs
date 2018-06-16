using System;
using System.Threading.Tasks;

namespace RemoteFork.Updater {
    public class Updater {
        private readonly GithubProvider _releaseProvider;

        public bool FindUpdate { get; private set; }

        private readonly Version _installedVersion;

        public Updater(GithubProvider releaseProvider, string installedVersion) {
            _installedVersion = new Version(installedVersion);
            _releaseProvider = releaseProvider;
        }

        public async Task CheckUpdate() {
            var latestVersion = new Version(await _releaseProvider.GetLatestVersionNumber());
            if (latestVersion > _installedVersion) {
                FindUpdate = true;
            }
        }

        public string GetDownloadLinkLastVersion() {
            return _releaseProvider.GetDownloadLinkLastVersion();
        }

        public async Task<string> GetLatestVersionNumber(bool force = true) {
            return await _releaseProvider.GetLatestVersionNumber(force);
        }
    }
}
