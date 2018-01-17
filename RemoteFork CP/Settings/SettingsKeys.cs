namespace RemoteFork.Settings {
    public static class SettingsKey {
        public const string IP_ADDRESS = "IpIPAddress";

        public const string PORT = "Port";
        //public const string UseProxy = "UseProxy";

        public const string USER_AGENT = "UserAgent";

        //public const string AutoStartWindows = "AutoStartWindows";
        //public const string AutoStartServer = "AutoStartServer";

        public const string DLNA = "Dlna";
        public const string DLNA_FILTER_TYPE = "DlnaFilterType";
        public const string DLNA_DIRECTORIES = "DlnaDirectories";
        public const string DLNA_FILE_EXTENSIONS = "DlnaFileExtensions";
        public const string DLNA_HIIDEN_FILES = "DlnaHiidenFiles";
        public const string FILE_BUFFER_SIZE = "FileBufferSize";

        public const string PLUGINS = "Plugins";
        public const string ENABLE_PLUGINS = "EnablePlugins";

        public const string USER_URLS = "UserUrls";

        //public const string THVPAutoStart = "THVPAutoStart";
        public const string ACE_STREAM_PORT = "AceStreamPort";

        public const string LOG_LEVEL = "LogLevel";
        public const string CHECK_UPDATE = "CheckUpdate";
    }

    public enum FilterMode : byte {
        NONE = 0,
        INCLUSION = 1,
        EXCLUSION = 2
    }
}
