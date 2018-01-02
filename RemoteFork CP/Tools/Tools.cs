using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace RemoteFork {
    internal static class Tools {

        #region CheckAccess

        public static bool CheckAccessPath(DirectoryInfo directory) {
            return !CheckHiddenFile(directory.Attributes) && CheckAccessPath(directory.FullName);
        }

        public static bool CheckAccessPath(FileInfo file) {
            return !CheckHiddenFile(file.Attributes) && CheckAccessPath(file.FullName);
        }

        public static bool CheckAccessPath(string file) {
            bool result = true;

            file = Path.GetFullPath(file);

            if (SettingsManager.Settings.DlnaDirectories != null) {
                var filter = new List<string>(SettingsManager.Settings.DlnaDirectories);
                switch (SettingsManager.Settings.DlnaFilterType) {
                    case SettingsManager.FilterMode.INCLUSION:
                        if (filter.All(i => !file.StartsWith(i, StringComparison.OrdinalIgnoreCase))) {
                            result = false;
                        }
                        break;
                    case SettingsManager.FilterMode.EXCLUSION:
                        if (filter.Any(file.StartsWith)) {
                            result = false;
                        }
                        break;
                }
            }

            if (File.Exists(file)) {
                if ((SettingsManager.Settings.DlnaFileExtensions != null) && (SettingsManager.Settings.DlnaFileExtensions.Length > 0)) {
                    result = SettingsManager.Settings.DlnaFileExtensions.Any(file.EndsWith);
                }
            }

            return result;
        }

        public static bool CheckHiddenFile(FileAttributes attributes) {
            return !SettingsManager.Settings.DlnaHiidenFiles &&
                   (((attributes & FileAttributes.Hidden) == FileAttributes.Hidden) ||
                    ((attributes & FileAttributes.System) == FileAttributes.System));
        }

        #endregion CheckAccess

        public static IPAddress[] GetIPAddresses(string hostname = "") {
            var hostEntry = Dns.GetHostEntry(hostname);
            var addressList = hostEntry.AddressList;
            return (from iPAddress in addressList
                    let flag = iPAddress.AddressFamily == AddressFamily.InterNetwork
                    where flag
                    select iPAddress)
                .ToArray();
        }

        public static string FSize(long len) {
            float num = len;
            string str = "Байт";
            bool flag = num > 102f;
            if (flag) {
                num /= 1024f;
                str = "КБ";
            }
            bool flag2 = num > 102f;
            if (flag2) {
                num /= 1024f;
                str = "МБ";
            }
            bool flag3 = num > 102f;
            if (flag3) {
                num /= 1024f;
                str = "ГБ";
            }
            return Math.Round(num, 2) + str;
        }

        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
            if (val.CompareTo(min) < 0) return min;
            return val.CompareTo(max) > 0 ? max : val;
        }
    }
}