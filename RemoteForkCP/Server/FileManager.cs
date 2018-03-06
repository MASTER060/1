using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace RemoteFork.Server {
    public static class FileManager {

        public static bool DirectoryExists(string path) {
            return Directory.Exists(path);
        }

        public static IDictionary<string, string> GetDirectories(string path) {
            return GetInfos(path, true);
        }

        public static IDictionary<string, string> GetFiles(string path) {
            return GetInfos(path, false);
        }

        public static IDictionary<string, string> GetInfos(string path, bool isDirectory) {
            var physicalProvider = new PhysicalFileProvider(path);

            var files = new SortedDictionary<string, string>();

            var contents = physicalProvider.GetDirectoryContents("");

            foreach (var content in contents) {
                if (content.IsDirectory == isDirectory) {
                    files.Add(content.PhysicalPath, content.Name);
                }
            }

            return files;
        }

        public static DriveInfo[] GetDrives() {
            return DriveInfo.GetDrives();
        }
    }
}
