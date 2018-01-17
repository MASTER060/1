using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using RemoteFork;
using RemoteFork.Plugins;
using RemoteFork.Settings;
using RemoteFork_CP.Models;

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

            ViewBag.Ips = new SelectList(ipList, "Value", "Text", ProgramSettings.Settings.IpAddress);
            ViewBag.logs = new SelectList(logList, "Value", "Text", ProgramSettings.Settings.LogLevel);

            ViewData["Port"] = ProgramSettings.Settings.Port;
            ViewData["UseProxy"] = false;
            //ViewData["THVPAutoStart"] = ProgramSettings.Settings.THVPAutoStart;
            ViewData["AceStreamPort"] = ProgramSettings.Settings.AceStreamPort;
            ViewData["LogLevel"] = ProgramSettings.Settings.LogLevel;
            ViewData["CheckUpdate"] = ProgramSettings.Settings.CheckUpdate;
            ViewData["UserAgent"] = ProgramSettings.Settings.UserAgent;
            return View();
        }

        [HttpPost]
        public IActionResult Index(SettingsModel settings) {
            if (!string.IsNullOrEmpty(settings.IP)) {
                if (ProgramSettings.Settings.IpAddress != settings.IP) {
                    ProgramSettings.Settings.IpAddress = settings.IP;
                }
                if (!string.IsNullOrEmpty(settings.UserAgent)) {
                    if (ProgramSettings.Settings.UserAgent != settings.UserAgent) {
                        ProgramSettings.Settings.UserAgent = settings.UserAgent;
                    }
                }
                if (!string.IsNullOrEmpty(settings.Port)) {
                    ushort.TryParse(settings.Port, out ushort value);
                    if (ProgramSettings.Settings.Port != value) {
                        ProgramSettings.Settings.Port = value;
                    }
                }
                if (!string.IsNullOrEmpty(settings.CheckUpdate)) {
                    bool value = settings.CheckUpdate == "on";
                    if (ProgramSettings.Settings.CheckUpdate != value) {
                        ProgramSettings.Settings.CheckUpdate = value;
                    }
                } else if (ProgramSettings.Settings.CheckUpdate) {
                    ProgramSettings.Settings.CheckUpdate = false;
                }
            }
            //if (!string.IsNullOrEmpty(settings.UseProxy)) {
            //    bool value = settings.UseProxy == "on";
            //    if (ProgramSettings.Settings.UseProxy != value) {
            //        ProgramSettings.Settings.UseProxy = value;
            //    }
            //} else if (ProgramSettings.Settings.UseProxy) {
            //    ProgramSettings.Settings.UseProxy = false;
            //}
            if (!string.IsNullOrEmpty(settings.AceStreamPort)) {
                ushort.TryParse(settings.AceStreamPort, out ushort value);
                if (ProgramSettings.Settings.AceStreamPort != value) {
                    ProgramSettings.Settings.AceStreamPort = value;
                }
            }
            if (!string.IsNullOrEmpty(settings.Log)) {
                byte value;
                byte.TryParse(settings.Log, out value);
                if (ProgramSettings.Settings.LogLevel != value) {
                    ProgramSettings.Settings.LogLevel = value;
                }
            }
            ProgramSettings.SettingsManager.Save();
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

            ProgramSettings.SettingsManager.Save();
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
            ViewBag.FilterMode = new SelectList(filterList, "Value", "Text", ((byte) ProgramSettings.Settings.DlnaFilterType).ToString());

            //ViewData["Enable"] = ProgramSettings.Settings.Dlna;
            //ViewData["HiidenFiles"] = ProgramSettings.Settings.DlnaHiidenFiles;
            //ViewData["Directories"] = string.Join(Environment.NewLine, ProgramSettings.Settings.DlnaDirectories);
            //ViewData["FileExtensions"] = string.Join(", ", ProgramSettings.Settings.DlnaFileExtensions);

            var model = new DlnaModel() {
                Directories = string.Join(Environment.NewLine, ProgramSettings.Settings.DlnaDirectories),
                Enable = ProgramSettings.Settings.Dlna,
                FileExtensions = string.Join(", ", ProgramSettings.Settings.DlnaFileExtensions),
                FilterMode = ((byte)ProgramSettings.Settings.DlnaFilterType).ToString(),
                HiidenFiles = ProgramSettings.Settings.DlnaHiidenFiles
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Dlna(DlnaModel settings) {
            if (ProgramSettings.Settings.Dlna != settings.Enable) {
                ProgramSettings.Settings.Dlna = settings.Enable;
            }
            if (ProgramSettings.Settings.DlnaHiidenFiles != settings.HiidenFiles) {
                ProgramSettings.Settings.DlnaHiidenFiles = settings.HiidenFiles;
            }
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

            ProgramSettings.SettingsManager.Save();
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
            ViewData["Enable"] = ProgramSettings.Settings.Plugins;
            ViewData["Plugins"] = string.Join(Environment.NewLine, ProgramSettings.Settings.EnablePlugins);
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
                EnablePlugins = ProgramSettings.Settings.EnablePlugins
            });
        }

        [HttpPost]
        public IActionResult Plugins(PluginsModel settings) {
            if (!string.IsNullOrEmpty(settings.Enable)) {
                bool value = settings.Enable == "on";
                if (ProgramSettings.Settings.Plugins != value) {
                    ProgramSettings.Settings.Plugins = value;
                }
            } else if (ProgramSettings.Settings.Plugins) {
                ProgramSettings.Settings.Plugins = false;
            }
            if (settings.EnablePlugins != null && settings.EnablePlugins.Any()) {
                ProgramSettings.Settings.EnablePlugins = settings.EnablePlugins.ToArray();
            } else {
                ProgramSettings.Settings.EnablePlugins = new string[0];
            }

            ProgramSettings.SettingsManager.Save();
            return Plugins();
        }

        #endregion PLUGINS_SETTINGS

        public IActionResult Error() {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
