using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using RemoteFork.Log.Analytics;
using RemoteFork.Models;
using RemoteFork.Network;
using RemoteFork.Plugins;
using RemoteFork.Settings;

namespace RemoteFork.Controllers {
    [GoogleAnalyticsTrackEvent]
    public class HomeController : Controller {
        #region MAIN_SETTINGS

        [HttpGet]
        public IActionResult Index() {
            var ipAddresses = Tools.Tools.GetIPAddresses();
            var ipList = ipAddresses.Select(ip => new SelectListItem() {
                    Text = ip.ToString(),
                    Value = ip.ToString()
                })
                .ToList();

            var logLevels = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>();
            var logList = logLevels.Select(log => new SelectListItem() {
                    Text = log.ToString(),
                    Value = ((byte) log).ToString()
                })
                .ToList();

            ViewBag.Ips = new SelectList(ipList, "Value", "Text", ProgramSettings.Settings.IpAddress);
            ViewBag.logs = new SelectList(logList, "Value", "Text", ProgramSettings.Settings.LogLevel);

            ViewData["AceStreamCheck"] =
                $"http://{ProgramSettings.Settings.IpAddress}:{ProgramSettings.Settings.AceStreamPort}/webui/api/service?method=get_version&format=jsonp&callback=mycallback";

            var model = new SettingsModel() {
                IP = ProgramSettings.Settings.IpAddress,
                Port = ProgramSettings.Settings.Port,
                ListenLocalhost = ProgramSettings.Settings.ListenLocalhost,

                ProxyEnable = ProgramSettings.Settings.UseProxy,
                ProxyAddress = ProgramSettings.Settings.ProxyAddress,
                ProxyUserName = ProgramSettings.Settings.ProxyUserName,
                ProxyPassword = ProgramSettings.Settings.ProxyPassword,
                ProxyNotDefaultEnable = ProgramSettings.Settings.ProxyNotDefaultEnable,

                AceStreamPort = ProgramSettings.Settings.AceStreamPort,
                CheckUpdate = ProgramSettings.Settings.CheckUpdate,
                UserAgent = ProgramSettings.Settings.UserAgent,
                DeveloperMode = ProgramSettings.Settings.DeveloperMode
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SettingsModel settings) {
            if (!string.IsNullOrEmpty(settings.IP)) {
                ProgramSettings.Settings.IpAddress = settings.IP;

                if (!string.IsNullOrEmpty(settings.UserAgent)) {
                    ProgramSettings.Settings.UserAgent = settings.UserAgent;
                }

                ProgramSettings.Settings.ListenLocalhost = settings.ListenLocalhost;

                ProgramSettings.Settings.CheckUpdate = settings.CheckUpdate;
                ProgramSettings.Settings.DeveloperMode = settings.DeveloperMode;

                ProgramSettings.Settings.UseProxy = settings.ProxyEnable;
                ProgramSettings.Settings.ProxyAddress = settings.ProxyAddress;
                ProgramSettings.Settings.ProxyUserName = settings.ProxyUserName;
                ProgramSettings.Settings.ProxyPassword = settings.ProxyPassword;
                ProgramSettings.Settings.ProxyNotDefaultEnable = settings.ProxyNotDefaultEnable;

                if (settings.ProxyEnable) {
                    HTTPUtility.CreateProxy(ProgramSettings.Settings.ProxyAddress,
                        ProgramSettings.Settings.ProxyUserName, ProgramSettings.Settings.ProxyPassword);
                } else {
                    HTTPUtility.CreateProxy();
                }
            }

            if (ProgramSettings.Settings.AceStreamPort != settings.AceStreamPort) {
                ProgramSettings.Settings.AceStreamPort = settings.AceStreamPort;
            }

            if (!string.IsNullOrEmpty(settings.Log)) {
                byte.TryParse(settings.Log, out byte value);
                if (ProgramSettings.Settings.LogLevel != value) {
                    ProgramSettings.Settings.LogLevel = value;
                }
            }

            ProgramSettings.Instance.Save();
            return Index();
        }

        #endregion MAIN_SETTINGS

        #region USER_LINKS_SETTINGS

        [HttpGet]
        public IActionResult UserLinks() {
            var model = new UserLinksModel() {
                Links = string.Join(Environment.NewLine, ProgramSettings.Settings.UserUrls)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult UserLinks(UserLinksModel settings) {
            ProgramSettings.Settings.UserUrls = !string.IsNullOrEmpty(settings.Links)
                ? settings.Links.Split(Environment.NewLine)
                : new string[0];

            ProgramSettings.Instance.Save();
            return UserLinks();
        }

        #endregion USER_LINKS_SETTINGS

        #region DLNA_SETTINGS

        [HttpGet]
        public IActionResult Dlna() {
            var filterMode = Enum.GetValues(typeof(FilterMode)).Cast<FilterMode>();
            var filterList = filterMode.Select(log => new SelectListItem() {
                    Text = log.ToString(),
                    Value = ((byte) log).ToString()
                })
                .ToList();
            ViewBag.FilterMode = new SelectList(filterList, "Value", "Text",
                ((byte) ProgramSettings.Settings.DlnaFilterType).ToString());

            var model = new DlnaModel() {
                Directories = string.Join(Environment.NewLine, ProgramSettings.Settings.DlnaDirectories),
                Enable = ProgramSettings.Settings.Dlna,
                FileExtensions = string.Join(", ", ProgramSettings.Settings.DlnaFileExtensions),
                FilterMode = ((byte) ProgramSettings.Settings.DlnaFilterType).ToString(),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Dlna(DlnaModel settings) {
            ProgramSettings.Settings.Dlna = settings.Enable;

            if (!string.IsNullOrEmpty(settings.FilterMode)) {
                Enum.TryParse(settings.FilterMode, out FilterMode value);
                if (ProgramSettings.Settings.DlnaFilterType != value) {
                    ProgramSettings.Settings.DlnaFilterType = value;
                }
            }

            ProgramSettings.Settings.DlnaDirectories = !string.IsNullOrEmpty(settings.Directories)
                ? settings.Directories.Split(Environment.NewLine)
                : new string[0];
            ProgramSettings.Settings.DlnaFileExtensions = !string.IsNullOrEmpty(settings.FileExtensions)
                ? settings.FileExtensions.Replace(" ", "").Split(",")
                : new string[0];

            ProgramSettings.Instance.Save();
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

            var pluginsList = new List<SelectListItem>();
            var allPlugins = PluginManager.Instance.GetPlugins(false);
            foreach (var plugin in allPlugins) {
                pluginsList.Add(new SelectListItem() {
                    Text = plugin.Value.ToString(),
                    Value = plugin.Value.Key
                });
            }

            return View(new PluginsModel() {
                Enable = ProgramSettings.Settings.Plugins,
                Plugins = pluginsList,
                EnablePlugins = ProgramSettings.Settings.EnablePlugins
            });
        }

        [HttpPost]
        public IActionResult Plugins(PluginsModel settings) {
            ProgramSettings.Settings.Plugins = settings.Enable;
            if (settings.EnablePlugins != null && settings.EnablePlugins.Any()) {
                ProgramSettings.Settings.EnablePlugins = settings.EnablePlugins.ToArray();
            } else {
                ProgramSettings.Settings.EnablePlugins = new string[0];
            }

            ProgramSettings.Instance.Save();
            return Plugins();
        }

        #endregion PLUGINS_SETTINGS

        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
