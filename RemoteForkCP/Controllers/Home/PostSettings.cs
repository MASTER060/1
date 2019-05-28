using System;
using RemoteFork.Models;
using RemoteFork.Network;
using RemoteFork.Settings;

namespace RemoteFork.Controllers.Home {
    public class PostSettings {
        public static void PostModel(SettingsModel settings) {
            if (!string.IsNullOrEmpty(settings.IP)) {
                ProgramSettings.Settings.IpAddress = settings.IP;

                if (!string.IsNullOrEmpty(settings.UserAgent)) {
                    ProgramSettings.Settings.UserAgent = settings.UserAgent;
                }

                ProgramSettings.Settings.ListenLocalhost = settings.ListenLocalhost;

                ProgramSettings.Settings.CheckUpdate = settings.CheckUpdate;
                ProgramSettings.Settings.DeveloperMode = settings.DeveloperMode;

                ProgramSettings.Settings.StartPageModernStyle = settings.StartPageModernStyle;

                if (!string.IsNullOrEmpty(settings.ProxyType)) {
                    Enum.TryParse(settings.ProxyType, out ProxyType value);
                    if (ProgramSettings.Settings.ProxyType != value) {
                        ProgramSettings.Settings.ProxyType = value;
                    }
                }

                ProgramSettings.Settings.UseProxy = settings.ProxyEnable;
                ProgramSettings.Settings.ProxyAddress = settings.ProxyAddress;
                ProgramSettings.Settings.ProxyPort = settings.ProxyPort;
                ProgramSettings.Settings.ProxyUserName = settings.ProxyUserName;
                ProgramSettings.Settings.ProxyPassword = settings.ProxyPassword;
                ProgramSettings.Settings.ProxyNotDefaultEnable = settings.ProxyNotDefaultEnable;

                if (settings.ProxyEnable) {
                    HTTPUtility.CreateProxy(ProgramSettings.Settings.ProxyAddress, ProgramSettings.Settings.ProxyPort,
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
        }
    }
}
