using Microsoft.AspNetCore.Mvc;
using RemoteFork.Controllers.Home;
using RemoteFork.Log.Analytics;
using RemoteFork.Models;
using RemoteFork.Plugins;
using System.Diagnostics;

namespace RemoteFork.Controllers {
    [GoogleAnalyticsTrackEvent]
    public class HomeController : Controller {
        #region MAIN_SETTINGS

        [HttpGet]
        public IActionResult Index() {
            var model = GetSettings.GetModel(ViewBag);

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SettingsModel settings) {
            PostSettings.PostModel(settings);
            return Index();
        }

        #endregion MAIN_SETTINGS

        #region USER_LINKS_SETTINGS

        [HttpGet]
        public IActionResult UserLinks() {
            var model = GetUserLinks.GetModel(ViewBag);
            return View(model);
        }

        [HttpPost]
        public IActionResult UserLinks(UserLinksModel settings) {
            PostUserLinks.PostModel(settings);
            return UserLinks();
        }

        #endregion USER_LINKS_SETTINGS

        #region DLNA_SETTINGS

        [HttpGet]
        public IActionResult Dlna() {
            var model = GetDlna.GetModel(ViewBag);

            return View(model);
        }

        [HttpPost]
        public IActionResult Dlna(DlnaModel settings) {
            PostDlna.PostModel(settings);
            return Dlna();
        }

        #endregion DLNA_SETTINGS

        #region PLUGINS_SETTINGS

        [HttpGet]
        public IActionResult Plugins() {
            if (Request.QueryString.HasValue) {
                if (Request.Query.ContainsKey("action")) {
                    if (Request.Query["action"] == "reimport") {
                        PluginManager.Instance.ReimportPlugins();
                    }
                }
            }

            var model = GetPlugins.GetModel(ViewBag);
            return View(model);
        }

        [HttpPost]
        public IActionResult Plugins(PluginsModel settings) {
            PostPlugins.PostModel(settings);
            return Plugins();
        }

        #endregion PLUGINS_SETTINGS

        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
