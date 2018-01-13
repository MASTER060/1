using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using RemoteFork;
using RemoteFork.Plugins;
using RemoteFork_CP.Models;
using static RemoteFork.SettingsManager;

namespace RemoteFork_CP.Controllers {
    public class HomeController : Controller {

        #region MAIN_SETTINGS

        [HttpGet]
        public IActionResult Index() {
            var ipAddresses = Tools.GetIPAddresses();
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

            ViewBag.Ips = new SelectList(ipList, "Value", "Text", SettingsManager.Settings.IpAddress);
            ViewBag.logs = new SelectList(logList, "Value", "Text", SettingsManager.Settings.LogLevel);

            ViewData["Port"] = SettingsManager.Settings.Port;
            ViewData["UseProxy"] = SettingsManager.Settings.UseProxy;
            //ViewData["THVPAutoStart"] = SettingsManager.Settings.THVPAutoStart;
            ViewData["AceStreamPort"] = SettingsManager.Settings.AceStreamPort;
            ViewData["LogLevel"] = SettingsManager.Settings.LogLevel;
            ViewData["CheckUpdate"] = SettingsManager.Settings.CheckUpdate;
            ViewData["UserAgent"] = SettingsManager.Settings.UserAgent;
            return View();
        }

        [HttpPost]
        public IActionResult Index(SettingsModel settings) {
            if (!string.IsNullOrEmpty(settings.IP)) {
                if (SettingsManager.Settings.IpAddress != settings.IP) {
                    SettingsManager.Settings.IpAddress = settings.IP;
                }
            }
            if (!string.IsNullOrEmpty(settings.UserAgent)) {
                if (SettingsManager.Settings.UserAgent != settings.UserAgent) {
                    SettingsManager.Settings.UserAgent = settings.UserAgent;
                }
            }
            if (!string.IsNullOrEmpty(settings.Port)) {
                ushort.TryParse(settings.Port, out ushort value);
                if (SettingsManager.Settings.Port != value) {
                    SettingsManager.Settings.Port = value;
                }
            }
            if (!string.IsNullOrEmpty(settings.CheckUpdate)) {
                bool value = settings.CheckUpdate == "on";
                if (SettingsManager.Settings.CheckUpdate != value) {
                    SettingsManager.Settings.CheckUpdate = value;
                }
            } else if (SettingsManager.Settings.CheckUpdate) {
                SettingsManager.Settings.CheckUpdate = false;
            }
            if (!string.IsNullOrEmpty(settings.UseProxy)) {
                bool value = settings.UseProxy == "on";
                if (SettingsManager.Settings.UseProxy != value) {
                    SettingsManager.Settings.UseProxy = value;
                }
            } else if (SettingsManager.Settings.UseProxy) {
                SettingsManager.Settings.UseProxy = false;
            }
            if (!string.IsNullOrEmpty(settings.AceStreamPort)) {
                ushort.TryParse(settings.AceStreamPort, out ushort value);
                if (SettingsManager.Settings.AceStreamPort != value) {
                    SettingsManager.Settings.AceStreamPort = value;
                }
            }
            if (!string.IsNullOrEmpty(settings.Log)) {
                byte value;
                byte.TryParse(settings.Log, out value);
                if (SettingsManager.Settings.LogLevel != value) {
                    SettingsManager.Settings.LogLevel = value;
                }
            }
            Save();
            return Index();
        }

        #endregion MAIN_SETTINGS

        #region USER_LINKS_SETTINGS

        [HttpGet]
        public IActionResult UserLinks() {
            var model = new UserLinksModel() {
                Links = string.Join(Environment.NewLine, SettingsManager.Settings.UserUrls)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult UserLinks(UserLinksModel settings) {
            if (!string.IsNullOrEmpty(settings.Links)) {
                SettingsManager.Settings.UserUrls = settings.Links.Split(Environment.NewLine);
            } else {
                SettingsManager.Settings.UserUrls = new string[0];
            }

            Save();
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
            ViewBag.FilterMode = new SelectList(filterList, "Value", "Text", ((byte) SettingsManager.Settings.DlnaFilterType).ToString());

            //ViewData["Enable"] = SettingsManager.Settings.Dlna;
            //ViewData["HiidenFiles"] = SettingsManager.Settings.DlnaHiidenFiles;
            //ViewData["Directories"] = string.Join(Environment.NewLine, SettingsManager.Settings.DlnaDirectories);
            //ViewData["FileExtensions"] = string.Join(", ", SettingsManager.Settings.DlnaFileExtensions);

            var model = new DlnaModel() {
                Directories = string.Join(Environment.NewLine, SettingsManager.Settings.DlnaDirectories),
                Enable = SettingsManager.Settings.Dlna,
                FileExtensions = string.Join(", ", SettingsManager.Settings.DlnaFileExtensions),
                FilterMode = ((byte)SettingsManager.Settings.DlnaFilterType).ToString(),
                HiidenFiles = SettingsManager.Settings.DlnaHiidenFiles
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Dlna(DlnaModel settings) {
            if (SettingsManager.Settings.Dlna != settings.Enable) {
                SettingsManager.Settings.Dlna = settings.Enable;
            }
            if (SettingsManager.Settings.DlnaHiidenFiles != settings.HiidenFiles) {
                SettingsManager.Settings.DlnaHiidenFiles = settings.HiidenFiles;
            }
            if (!string.IsNullOrEmpty(settings.FilterMode)) {
                Enum.TryParse(settings.FilterMode, out FilterMode value);
                if (SettingsManager.Settings.DlnaFilterType != value) {
                    SettingsManager.Settings.DlnaFilterType = value;
                }
            }
            SettingsManager.Settings.DlnaDirectories = !string.IsNullOrEmpty(settings.Directories)
                ? settings.Directories.Split(Environment.NewLine)
                : new string[0];
            SettingsManager.Settings.DlnaFileExtensions = !string.IsNullOrEmpty(settings.FileExtensions)
                ? settings.FileExtensions.Replace(" ", "").Split(",")
                : new string[0];

            Save();
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
            ViewData["Enable"] = SettingsManager.Settings.Plugins;
            ViewData["Plugins"] = string.Join(Environment.NewLine, SettingsManager.Settings.EnablePlugins);
            var pluginsList = new List<SelectListItem> ();
            var allPlugins = PluginManager.Instance.GetPlugins(false);
            foreach (var plugin in allPlugins) {
                pluginsList.Add(new SelectListItem() {
                    Text = plugin.Value.ToString(),
                    Value = plugin.Value.Key
                });
            }
            return View(new PluginsModel() {
                Plugins = pluginsList,
                EnablePlugins = SettingsManager.Settings.EnablePlugins
            });
        }

        [HttpPost]
        public IActionResult Plugins(PluginsModel settings) {
            if (!string.IsNullOrEmpty(settings.Enable)) {
                bool value = settings.Enable == "on";
                if (SettingsManager.Settings.Plugins != value) {
                    SettingsManager.Settings.Plugins = value;
                }
            } else if (SettingsManager.Settings.Plugins) {
                SettingsManager.Settings.Plugins = false;
            }
            if (settings.EnablePlugins != null && settings.EnablePlugins.Any()) {
                SettingsManager.Settings.EnablePlugins = settings.EnablePlugins.ToArray();
            } else {
                SettingsManager.Settings.EnablePlugins = new string[0];
            }

            Save();
            return Plugins();
        }

        #endregion PLUGINS_SETTINGS

        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
