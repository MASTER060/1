using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using RemoteFork.Models;
using RemoteFork.Settings;
using System;
using System.Linq;
using RemoteFork.Updater;

namespace RemoteFork.Controllers.Home {
    public static class GetSettings {
        public static SettingsModel GetModel(dynamic viewBag) {
            var ipAddresses = Tools.Tools.GetIPAddresses();
            var ipList = ipAddresses.Select(ip => new SelectListItem() {
                Text = ip.ToString(),
                Value = ip.ToString()
            }).ToList();

            var logLevels = Enum.GetValues(typeof(LogLevel)).Cast<LogLevel>();
            var logList = logLevels.Select(log => new SelectListItem() {
                Text = log.ToString(),
                Value = ((byte) log).ToString()
            }).ToList();

            viewBag.Ips = new SelectList(ipList, "Value", "Text", ProgramSettings.Settings.IpAddress);
            viewBag.Logs = new SelectList(logList, "Value", "Text", ProgramSettings.Settings.LogLevel);

            viewBag.AceStreamCheck =
                $"http://{ProgramSettings.Settings.IpAddress}:{ProgramSettings.Settings.AceStreamPort}/webui/api/service?method=get_version&format=jsonp&callback=mycallback";

            var proxyType = Enum.GetValues(typeof(ProxyType)).Cast<ProxyType>();
            var proxyTypeList = proxyType.Select(proxy => new SelectListItem() {
                    Text = proxy.ToString(),
                    Value = ((byte)proxy).ToString()
                })
                .ToList();
            viewBag.ProxyType = new SelectList(proxyTypeList, "Value", "Text",
                ((byte)ProgramSettings.Settings.ProxyType).ToString());

            var model = new SettingsModel() {
                IP = ProgramSettings.Settings.IpAddress,
                Port = ProgramSettings.Settings.Port,
                ListenLocalhost = ProgramSettings.Settings.ListenLocalhost,

                ProxyEnable = ProgramSettings.Settings.UseProxy,
                ProxyAddress = ProgramSettings.Settings.ProxyAddress,
                ProxyPort = ProgramSettings.Settings.ProxyPort,
                ProxyUserName = ProgramSettings.Settings.ProxyUserName,
                ProxyPassword = ProgramSettings.Settings.ProxyPassword,
                ProxyNotDefaultEnable = ProgramSettings.Settings.ProxyNotDefaultEnable,
                ProxyType = ((byte)ProgramSettings.Settings.ProxyType).ToString(),

                AceStreamPort = ProgramSettings.Settings.AceStreamPort,
                CheckUpdate = ProgramSettings.Settings.CheckUpdate,
                UserAgent = ProgramSettings.Settings.UserAgent,
                DeveloperMode = ProgramSettings.Settings.DeveloperMode,

                StartPageModernStyle = ProgramSettings.Settings.StartPageModernStyle,
            };

            if (ProgramSettings.Settings.CheckUpdate) {
                if (UpdateController.IsUpdateAvaiable("RemoteFork")) {
                    var updater = UpdateController.GetUpdater("RemoteFork");
                    model.DownloadLink = updater.GetDownloadLinkLastVersion();
                    model.LatestVersion = updater.GetLatestVersionNumber(false).Result;
                }
            }

            return model;
        }
    }
}
