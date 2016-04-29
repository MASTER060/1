using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using RemoteFork.Properties;
using RemoteFork.Server;

namespace RemoteFork.Requestes {
    internal class DlnaBrowserRequest : BaseRequest {
        protected HttpProcessor processor;

        public DlnaBrowserRequest(string text, HttpProcessor processor) : base(text) {
            this.processor = processor;
        }

        public override string Execute() {
            string hostText = string.Format("http://{0}/", processor.Host);
            StringBuilder result = new StringBuilder();
            result.AppendLine("#EXTM3U");
            if (text == "/") {
                if (Settings.Default.DlnaFilterType == 1) {
                    if (Settings.Default.DlnaDirectories != null) {
                        foreach (string directory in Settings.Default.DlnaDirectories) {
                            if (Directory.Exists(directory)) {
                                string urlText = HttpUtility.UrlEncode(directory + "\\");

                                result.AppendLine(string.Format("#EXTINF:-1,{0}\n{1}treeview?{2}",
                                    directory,
                                    hostText, urlText));

                                Console.WriteLine(directory);
                            }
                        }
                    }
                } else {
                    var drives = DriveInfo.GetDrives();
                    List<string> filter = new List<string>();

                    if (Settings.Default.DlnaFilterType == 2) {
                        if (Settings.Default.DlnaDirectories != null) {
                            filter.AddRange(Settings.Default.DlnaDirectories.Cast<string>());
                        }
                    }

                    foreach (DriveInfo drive in drives.Where(i => filter.All(j => j != i.Name))) {
                        bool isReady = drive.IsReady;
                        if (isReady) {
                            string text5 = string.Format("{0} ({1} свободно из {2})", drive.Name,
                                Tools.FSize(drive.AvailableFreeSpace), Tools.FSize(drive.TotalSize));

                            string driveName = string.Format("{0}<br>Метка диска: {1}<br>Тип носителя: {2}", text5,
                                drive.VolumeLabel, drive.DriveType);

                            result.AppendLine(string.Format("#EXTINF:-1,{0}\n{1}treeview?{2}", driveName, hostText,
                                drive.Name));

                            Console.WriteLine(driveName);
                        }
                    }
                }
            } else {
                string[] array = Directory.GetDirectories(text);
                List<string> filter = new List<string>();

                if (Settings.Default.DlnaFilterType == 2) {
                    if (Settings.Default.DlnaDirectories != null) {
                        filter.AddRange(Settings.Default.DlnaDirectories.Cast<string>());
                    }
                }

                foreach (string directory in array.Where(i => filter.All(j => j != i))) {
                    string urlText = HttpUtility.UrlEncode(directory + "\\");

                    result.AppendLine(string.Format("#EXTINF:-1,{0}\n{1}treeview?{2}", directory.Split('\\').Last(),
                        hostText, urlText));

                    Console.WriteLine(directory);
                }
                array = Directory.GetFiles(text);
                foreach (string file in array) {
                    string urlText = HttpUtility.UrlEncode(file.Replace("\\", "\\\\"));
                    FileInfo fileInfo = new FileInfo(file);

                    result.AppendLine(string.Format("#EXTINF:-1,{0} ({1})\n{2}{3}", file.Split('\\').Last(),
                        Tools.FSize(fileInfo.Length), hostText, urlText));

                    Console.WriteLine(file);
                }
            }

            return result.ToString();
        }
    }
}
