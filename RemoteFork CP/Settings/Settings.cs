using Newtonsoft.Json;

namespace RemoteFork.Settings {
    public class Settings {
        [JsonProperty(SettingsKey.IP_ADDRESS)]
        public string IpAddress { get; set; }
        [JsonProperty(SettingsKey.PORT)]
        public ushort Port { get; set; }

        //[JsonProperty(SettingsManager.SettingsKey.UseProxy)]
        //public bool UseProxy { get; set; }

        [JsonProperty(SettingsKey.USER_AGENT)]
        public string UserAgent { get; set; }

        //[JsonProperty(SettingsManager.SettingsKey.AutoStartWindows)]
        //public bool AutoStartWindows { get; set; }
        //[JsonProperty(SettingsManager.SettingsKey.AutoStartServer)]
        //public bool AutoStartServer { get; set; }

        [JsonProperty(SettingsKey.DLNA)]
        public bool Dlna { get; set; }
        [JsonProperty(SettingsKey.DLNA_FILTER_TYPE)]
        public FilterMode DlnaFilterType { get; set; }
        [JsonProperty(SettingsKey.DLNA_DIRECTORIES)]
        public string[] DlnaDirectories { get; set; }
        [JsonProperty(SettingsKey.DLNA_FILE_EXTENSIONS)]
        public string[] DlnaFileExtensions { get; set; }
        [JsonProperty(SettingsKey.DLNA_HIIDEN_FILES)]
        public bool DlnaHiidenFiles { get; set; }
        [JsonProperty(SettingsKey.FILE_BUFFER_SIZE)]
        public int FileBufferSize { get; set; }

        [JsonProperty(SettingsKey.PLUGINS)]
        public bool Plugins { get; set; }
        [JsonProperty(SettingsKey.ENABLE_PLUGINS)]
        public string[] EnablePlugins { get; set; }

        [JsonProperty(SettingsKey.USER_URLS)]
        public string[] UserUrls { get; set; }

        //[JsonProperty(SettingsManager.SettingsKey.THVPAutoStart)]
        //public bool THVPAutoStart { get; set; }
        [JsonProperty(SettingsKey.ACE_STREAM_PORT)]
        public ushort AceStreamPort { get; set; }

        [JsonProperty(SettingsKey.LOG_LEVEL)]
        public byte LogLevel { get; set; }
        [JsonProperty(SettingsKey.CHECK_UPDATE)]
        public bool CheckUpdate { get; set; }

        public class StringArray {
            [JsonProperty]
            public string Value { get; set; }
        }
    }
}
