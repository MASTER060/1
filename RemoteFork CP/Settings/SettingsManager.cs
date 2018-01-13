using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RemoteFork {
    public class SettingsManager {
        //private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<SettingsManager>();

        private static Settings defaultSettings { get; } = new Settings() { 
                IpAddress = IPAddress.Parse("0.0.0.0").ToString(),
                Port = ((ushort)8027),
                UseProxy = false,

                UserAgent =
                "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36",

                //AutoStartWindows = false,
                //AutoStartServer = true,

                Dlna = true,
                DlnaFilterType = FilterMode.NONE,
                DlnaDirectories = (new string[0]),
                DlnaFileExtensions = (new string[0]),
                DlnaHiidenFiles = false,
                FileBufferSize = (int)262144,

                Plugins = true,
                EnablePlugins = new string[0],

                UserUrls = new string[0],

                //THVPAutoStart = true,
                AceStreamPort = 6878,

                LogLevel = (byte)LogLevel.Critical,
                CheckUpdate = true
            };

        public static class SettingsKey {
            public const string IpIPAddress = "IpIPAddress";
            public const string Port = "Port";
            public const string UseProxy = "UseProxy";

            public const string UserAgent = "UserAgent";

            public const string AutoStartWindows = "AutoStartWindows";
            public const string AutoStartServer = "AutoStartServer";

            public const string Dlna = "Dlna";
            public const string DlnaFilterType = "DlnaFilterType";
            public const string DlnaDirectories = "DlnaDirectories";
            public const string DlnaFileExtensions = "DlnaFileExtensions";
            public const string DlnaHiidenFiles = "DlnaHiidenFiles";
            public const string FileBufferSize = "FileBufferSize";

            public const string Plugins = "Plugins";
            public const string EnablePlugins = "EnablePlugins";

            public const string UserUrls = "UserUrls";

            public const string THVPAutoStart = "THVPAutoStart";
            public const string AceStreamPort = "AceStreamPort";

            public const string LogLevel = "LogLevel";
            public const string CheckUpdate = "CheckUpdate";
        }

        public enum FilterMode : byte {
            NONE = 0,
            INCLUSION = 1,
            EXCLUSION = 2
        }

        private static Settings _settings;
        
#if DEBUG
        private static readonly string FILE = Path.Combine(Environment.CurrentDirectory, "Settings.json");
#else
        private static readonly string FILE = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
#endif

        public static Settings Settings {
            get => _settings ?? (_settings = Load());
            set => _settings = value;
        }

        public SettingsManager() {
            Load();
        }

        public static void Save() {
            Save(JsonConvert.SerializeObject(Settings));
        }

        public static void Save(string json) {
            try {
                using (var stream = new StreamWriter(File.Open(FILE, FileMode.Create))) {
                    stream.Write(json);
                }
            } catch (IOException exception) {
                //Log.LogError(exception, exception.Message);
            } catch (JsonException exception) {
                //Log.LogError(exception, exception.Message);
            }
        }

        public static Settings Load() {
            try {
                if (!File.Exists(FILE)) {
                    Save(JsonConvert.SerializeObject(defaultSettings));
                }
                using (var stream = new StreamReader(File.OpenRead(FILE))) {
                    Settings = JsonConvert.DeserializeObject<Settings>(stream.ReadToEnd());
                }
            } catch (IOException exception) {
                //Log.LogError(exception, exception.Message);
            } catch (JsonException exception) {
                //Log.LogError(exception, exception.Message);
            }
            return _settings;
        }
    }
}
