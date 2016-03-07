using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using RemoteFork.Properties;

namespace RemoteFork {
    public class MyHttpServer : HttpServer {
        public MyHttpServer(IPAddress ip, int port) : base(ip, port) {
        }

        private void DlnaRequest(string httpUrl, HttpProcessor processor) {
            Console.WriteLine("video");
            using (
                var fileStream = new FileStream(System.Web.HttpUtility.UrlDecode(httpUrl.Substring(1)),
                    FileMode.Open, FileAccess.Read, FileShare.Read)) {
                try {
                    long num = -1L;
                    long num2;
                    if (processor.httpHeaders.Contains("Range")) {
                        string text = processor.httpHeaders["Range"].ToString().Replace("bytes=", "");
                        string[] array = text.Split('-');
                        num2 = long.Parse(array[0]);
                        if (array[1].Trim().Length > 0) {
                            long.TryParse(array[1], out num);
                        }
                        if (num == -1L) {
                            num = fileStream.Length;
                        }
                    } else {
                        num2 = 0L;
                        num = fileStream.Length;
                    }
                    processor.outputStream.AutoFlush = true;
                    processor.outputStream.WriteLine("HTTP/1.0 206 Partial Content");
                    processor.outputStream.WriteLine("Content-Type: video/mp4");
                    processor.outputStream.WriteLine("Accept-Ranges: bytes");
                    long num3 = num - num2;
                    processor.outputStream.WriteLine("Content-Range: bytes {0}-{1}/{2}", num2, fileStream.Length - 1L,
                        fileStream.Length);
                    processor.outputStream.WriteLine("Content-Length: " + num3);
                    processor.outputStream.WriteLine("Connection: Close");
                    processor.outputStream.WriteLine("");
                    processor.outputStream.AutoFlush = true;
                    long num4 = 20000L;
                    long num5 = num - num2;
                    Console.WriteLine("fs.Seek=" + num2);
                    fileStream.Seek(num2, SeekOrigin.Begin);
                    bool flag7 = true;
                    while (num5 > 0L) {
                        bool flag8 = flag7;
                        if (flag8) {
                            Console.WriteLine("starting Read, to_read={0}", num5);
                        }
                        flag7 = false;
                        byte[] buffer = new byte[num4];
                        int num6 = fileStream.Read(buffer, 0, (int) Math.Min(num4, num5));
                        if (num6 == 0) {
                            break;
                        }
                        num5 -= num6;
                        processor.outputStream.BaseStream.Write(buffer, 0, num6);
                    }
                } finally {
                    Console.WriteLine("fs read end ");
                    fileStream.Close();
                }
            }
        }

        private string TreeviewRequest(string text, HttpProcessor processor) {
            string hostText = "http://" + processor.httpHeaders["Host"] + "/";
            string result = "#EXTM3U\\n";
            if (text == "/") {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives) {
                    bool isReady = drive.IsReady;
                    if (isReady) {
                        string text5 = string.Format("{0} ({1} свободно из {2})", drive.Name,
                            Tools.FSize(drive.AvailableFreeSpace), Tools.FSize(drive.TotalSize));

                        string text6 = string.Format("{0}<br>Метка диска: {1}<br>Тип носителя: {2}", text5,
                            drive.VolumeLabel, drive.DriveType);

                        result = string.Format("{0}#EXTINF:-1,{1}\n{2}treeview?{3}\n", result, text6, hostText,
                            drive.Name);

                        Console.WriteLine(text6);
                    }
                }
            } else {
                string[] array = Directory.GetDirectories(text);
                foreach (string directory in array) {
                    string urlText = Uri.EscapeUriString(directory + "\\");
                    result = string.Format("{0}#EXTINF:-1,{1}\n{2}treeview?{3}\n", result, directory.Split('\\').Last(),
                        hostText, urlText);

                    Console.WriteLine(directory);
                }
                array = Directory.GetFiles(text);
                foreach (string file in array) {
                    string urlText = Uri.EscapeUriString(file);
                    FileInfo fileInfo = new FileInfo(file);
                    result = string.Format("{0}#EXTINF:-1,{1} {2}\n{3}{4}\n", result, file.Split('\\').Last(),
                        Tools.FSize(fileInfo.Length), hostText, urlText);

                    Console.WriteLine(file);
                }
            }

            return result;
        }

        private string TestRequest(string httpUrl) {
            string result =
                "<html><h1>ForkPlayer DLNA Work!</h1><br><b>Server by Visual Studio 2015</b></html>";
            if (httpUrl.IndexOf('|') > 0) {
                string device = httpUrl.Replace("/test?", "");
                if (!Main.Devices.Contains(device)) {
                    Main.Devices.Add(device);
                }
            }
            return result;
        }

        private string ParseCurlRequest(string httpUrl) {
            string result = string.Empty;

            string url = Regex.Match(httpUrl, "(?:\")(.*?)(?=\")").Groups[1].Value;
            Dictionary<string, string> header = new Dictionary<string, string>();
            MatchCollection matches = Regex.Matches(httpUrl, "(?:-H\\s\")(.*?)(?=\")");
            foreach (Match match in matches) {
                var groups = match.Groups;
                if (groups.Count > 1) {
                    var value = groups[1].Value;
                    if (value.Contains(": ")) {
                        header.Add(value.Remove(value.IndexOf(": ")), value.Substring(value.IndexOf(": ") + 2));
                    }
                }
            }
            if (httpUrl.Contains("--data")) {
                string dataString = Regex.Match(httpUrl, "(?:--data\\s\")(.*?)(?=\")").Groups[1].Value;
                var dataArray = dataString.Split('&');
                Dictionary<string, string> data = new Dictionary<string, string>();
                foreach (var value in dataArray) {
                    data.Add(value.Remove(value.IndexOf("=")), value.Substring(value.IndexOf("=") + 1));
                }
                result =
                    HttpUtility.PostRequest(url, data, header).Result;
            } else {
                result =
                    HttpUtility.GetRequest(url, header).Result;
            }

            return result;
        }

        private string ParserlinkRequest(string httpUrl) {
            string result = string.Empty;

            string text = System.Web.HttpUtility.UrlDecode(httpUrl.Substring(12));
            string[] array = text.Split('|');
            Console.WriteLine("parse0 " + array[0]);

            string response = array[0].StartsWith("curl")
                ? ParseCurlRequest(array[0])
                : HttpUtility.GetRequest(array[0]).Result;

            if (array.Length == 1) {
                result = response;
            } else {
                if (!array[1].Contains(".*?")) {
                    if (string.IsNullOrEmpty(array[1]) && string.IsNullOrEmpty(array[2])) {
                        result = response;
                    } else {
                        int num1 = response.IndexOf(array[1]);
                        if (num1 == -1) {
                            result = string.Empty;
                        } else {
                            num1 += array[1].Length;
                            int num2 = response.IndexOf(array[2], num1);
                            result = num2 == -1
                                ? string.Empty
                                : response.Substring(num1, num2 - num1);
                        }
                    }
                } else {
                    Console.WriteLine(array[1] + "(.*?)" + array[2]);
                    string pattern = array[1] + "(.*?)" + array[2];
                    Regex regex = new Regex(pattern, RegexOptions.Multiline);
                    Match match = regex.Match(response);
                    if (match.Success) {
                        result = match.Groups[1].Captures[0].ToString();
                    }
                }
            }

            return result;
        }

        public override void HandleGetRequest(HttpProcessor processor) {
            string httpUrl = System.Web.HttpUtility.UrlDecode(processor.http_url);
            string text = string.Empty;
            if (httpUrl.Length > 10) {
                text = httpUrl.Substring(10);
                if (httpUrl.IndexOf("&") > 0) {
                    text = text.Substring(0, text.IndexOf("&"));
                }
            }
            Console.WriteLine(httpUrl.Substring(1));
            if (File.Exists(httpUrl.Substring(1)) && Settings.Default.Dlna) {
                DlnaRequest(httpUrl, processor);
            } else {
                string result = string.Empty;
                if (httpUrl.StartsWith("/treeview") && Settings.Default.Dlna) {
                    result = TreeviewRequest(text, processor);
                } else {
                    if (httpUrl.StartsWith("/parserlink")) {
                        result = ParserlinkRequest(httpUrl);
                    } else {
                        if (httpUrl.StartsWith("/test")) {
                            result = TestRequest(httpUrl);
                        }
                    }
                }

                Console.WriteLine("request: {0}", processor.http_url);
                processor.WriteSuccess("text/html");
                processor.outputStream.WriteLine(result.Replace("WEB-DL", "WEBDL"));
                Console.WriteLine("List end");
            }
        }

        public override void HandlePostRequest(HttpProcessor processor, StreamReader inputData) {
            Console.WriteLine("POST request: {0}", processor.http_url);
            string arg = inputData.ReadToEnd();
            processor.WriteSuccess("text/html");
            processor.outputStream.WriteLine("<html><body><h1>test server</h1>");
            processor.outputStream.WriteLine("<a href=/test>return</a><p>");
            processor.outputStream.WriteLine("postbody: <pre>{0}</pre>", arg);
        }
    }
}
