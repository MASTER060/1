using Octokit;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemoteFork.Updater {
    /// <summary>
    ///     Provider for GitHubReleases
    /// </summary>
    public class GithubProvider {
        private readonly string _appName;
        private readonly string _repositoryOwner;
        private readonly string _repositoryName;
        private readonly string _releaseName;
        private readonly bool _checkOS;

        private Release _releaseAsset;

        /// <summary>
        ///     Create instance of GithubProvider. 
        /// </summary>
        /// <param name="appName">Name of current application.</param>
        /// <param name="url">Github repository link (format: "repositoryOwner/repositoryName/releaseName")</param>
        public GithubProvider(string appName, string url, bool checkOS) {
            _appName = appName;
            _repositoryOwner = url.Split("/")[0];
            _repositoryName = url.Split("/")[1];
            _releaseName = url.Split("/")[2].ToLower();
            _checkOS = checkOS;
        }

        public string GetDownloadLinkLastVersion() {
            if (_releaseAsset != null) {
                var asset = _releaseAsset.Assets.FirstOrDefault(i =>
                    !_checkOS || i.Name.StartsWith(OSVersion.GetOSVersion()));
                return asset?.BrowserDownloadUrl;
            } else {
                return string.Empty;
            }
        }

        public async Task<string> GetLatestVersionNumber(bool force = true) {
            if (force || _releaseAsset == null) {
                var client = new GitHubClient(new ProductHeaderValue(_appName));
                var releases = await client.Repository.Release.GetAll(_repositoryOwner, _repositoryName);
                var latest = releases.FirstOrDefault(i => i.Name.ToLower().Contains(_releaseName));
                if (latest != null) {
                    _releaseAsset = latest;
                }
            }

            if (_releaseAsset != null) {
                var regex = new Regex("(\\d+\\.?){3}(\\d+)");
                return regex.IsMatch(_releaseAsset.TagName)
                    ? regex.Match(_releaseAsset.TagName).Value
                    : _releaseAsset.TagName;
            }

            return "0";
        }
    }
}
