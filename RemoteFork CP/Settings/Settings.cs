using Newtonsoft.Json;

namespace RemoteFork {
    public class Settings {
        [JsonProperty(SettingsManager.SettingsKey.IpIPAddress)]
        public string IpIPAddress { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.Port)]
        public ushort Port { get; set; }

        [JsonProperty(SettingsManager.SettingsKey.UseProxy)]
        public bool UseProxy { get; set; }

        [JsonProperty(SettingsManager.SettingsKey.UserAgent)]
        public string UserAgent { get; set; }

        [JsonProperty(SettingsManager.SettingsKey.AutoStartWindows)]
        public bool AutoStartWindows { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.AutoStartServer)]
        public bool AutoStartServer { get; set; }

        [JsonProperty(SettingsManager.SettingsKey.Dlna)]
        public bool Dlna { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.DlnaFilterType)]
        public byte DlnaFilterType { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.DlnaDirectories)]
        public string[] DlnaDirectories { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.DlnaFileExtensions)]
        public string[] DlnaFileExtensions { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.DlnaHiidenFiles)]
        public bool DlnaHiidenFiles { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.FileBufferSize)]
        public int FileBufferSize { get; set; }

        [JsonProperty(SettingsManager.SettingsKey.Plugins)]
        public bool Plugins { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.EnablePlugins)]
        public string[] EnablePlugins { get; set; }

        [JsonProperty(SettingsManager.SettingsKey.UserUrls)]
        public string[] UserUrls { get; set; }

        [JsonProperty(SettingsManager.SettingsKey.THVPAutoStart)]
        public bool THVPAutoStart { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.AceStreamPort)]
        public ushort AceStreamPort { get; set; }

        [JsonProperty(SettingsManager.SettingsKey.LogLevel)]
        public byte LogLevel { get; set; }
        [JsonProperty(SettingsManager.SettingsKey.CheckUpdate)]
        public bool CheckUpdate { get; set; }

        public class StringArray {
            [JsonProperty]
            public string Value { get; set; }
        }
    }
}
