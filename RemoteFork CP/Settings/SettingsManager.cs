using System;
using System.IO;
using Newtonsoft.Json;

namespace RemoteFork.Settings {
    public class SettingsManager<T> {
        public T Settings { get; private set; }
        
#if DEBUG
        private static string PATH = Environment.CurrentDirectory;
#else
        private static string PATH = AppDomain.CurrentDomain.BaseDirectory;
#endif

        private readonly string _fileName;

        public SettingsManager(string fileName) {
            _fileName = Path.Combine(PATH, fileName);
            Load();
        }

        public void Save() {
            Save(JsonConvert.SerializeObject(Settings));
        }

        public void Save(T settings) {
            Settings = settings;
            Save(JsonConvert.SerializeObject(Settings));
        }

        private void Save(string json) {
            try {
                using (var stream = new StreamWriter(File.Open(_fileName, FileMode.Create))) {
                    stream.Write(json);
                }
            } catch (IOException exception) {
                //Log.LogError(exception, exception.Message);
            } catch (JsonException exception) {
                //Log.LogError(exception, exception.Message);
            }
        }

        private T Load() {
            try {
                using (var stream = new StreamReader(File.OpenRead(_fileName))) {
                    Settings = JsonConvert.DeserializeObject<T>(stream.ReadToEnd());
                }
            } catch (IOException exception) {
                //Log.LogError(exception, exception.Message);
            } catch (JsonException exception) {
                //Log.LogError(exception, exception.Message);
            }
            return Settings;
        }
    }
}
