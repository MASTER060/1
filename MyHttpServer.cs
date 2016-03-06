using System;
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
                var fileStream = new FileStream(WebUtility.UrlDecode(httpUrl.Substring(1)),
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
                        int num6 = fileStream.Read(buffer, 0, (int)Math.Min(num4, num5));
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
                for (int i = 0; i < drives.Length; i++) {
                    bool isReady = drives[i].IsReady;
                    if (isReady) {
                        string text5 = string.Format("{0} ({1} свободно из {2})", drives[i].Name,
                            Tools.FSize(drives[i].AvailableFreeSpace), Tools.FSize(drives[i].TotalSize));

                        string text6 = string.Format("{0}<br>Метка диска: {1}<br>Тип носителя: {2}", text5,
                            drives[i].VolumeLabel, drives[i].DriveType);

                        result = string.Format("{0}#EXTINF:-1,{1}\n{2}treeview?{3}\n", result, text6, hostText,
                            drives[i].Name);

                        Console.WriteLine(text6);
                    }
                }
            } else {
                string[] array = Directory.GetDirectories(text);
                for (int j = 0; j < array.Length; j++) {
                    string urlText = Uri.EscapeUriString(array[j] + "\\");
                    result = string.Format("{0}#EXTINF:-1,{1}\n{2}treeview?{3}\n", result, array[j].Split('\\').Last(),
                        hostText, urlText);

                    Console.WriteLine(array[j]);
                }
                array = Directory.GetFiles(text);
                for (int k = 0; k < array.Length; k++) {
                    string urlText = Uri.EscapeUriString(array[k]);
                    FileInfo fileInfo = new FileInfo(array[k]);
                    result = string.Format("{0}#EXTINF:-1,{1} {2}\n{3}{4}\n", result, array[k].Split('\\').Last(),
                        Tools.FSize(fileInfo.Length), hostText, urlText);

                    Console.WriteLine(array[k]);
                }
            }

            return result;
        }

        private string TestRequest(string httpUrl) {
            string result =
                "<html><h1>ForkPlayer DLNA Work!</h1>";
            if (httpUrl.IndexOf('|') > 0) {
                string text = httpUrl.Substring(6);
                Settings.Default.Devices = text.Replace('|', '~');
                Settings.Default.Save();
            }
            return result;
        }

        private string ParserlinkRequest(string httpUrl) {
            string result = string.Empty;

            string text = WebUtility.UrlDecode(httpUrl.Substring(12));
            string[] array = text.Split('|');
            Console.WriteLine("parse0 " + array[0]);
            if (array[0].IndexOf("curl") == 0) {
                array[0] = array[0].Remove(0, 4);
                if (array[0].Contains(' ')) {
                    array[0] = array[0].Remove(array[0].IndexOf(' '));
                }
                array[0] = array[0].Trim();
            }

            string text11 =
                HttpUtility.GetRequest(new Uri(array[0].Replace("\"", "\"\""))).Result;

            if (array.Length == 1) {
                result = text11;
            } else {
                if (array[1].IndexOf(".*?") == -1) {
                    if (array[1] == "" && array[2] == "") {
                        result = text11;
                    } else {
                        int num1 = text11.IndexOf(array[1]);
                        if (num1 == -1) {
                            result = "";
                        } else {
                            num1 += array[1].Length;
                            int num2 = text11.IndexOf(array[2], num1);
                            if (num2 == -1) {
                                result = "";
                            } else {
                                result = text11.Substring(num1, num2 - num1);
                            }
                        }
                    }
                } else {
                    Console.WriteLine(array[1] + "(.*?)" + array[2]);
                    string pattern = array[1] + "(.*?)" + array[2];
                    Regex regex = new Regex(pattern, RegexOptions.Multiline);
                    Match match = regex.Match(text11);
                    if (match.Success) {
                        result = match.Groups[1].Captures[0].ToString();
                    }
                }
            }

            return result;
        }

        public override void HandleGetRequest(HttpProcessor processor) {
            string httpUrl = processor.http_url;
            string text = string.Empty;
            if (httpUrl.Length > 10) {
                text = WebUtility.UrlDecode(httpUrl.Substring(10));
                if (httpUrl.IndexOf("&") > 0) {
                    text = text.Substring(0, text.IndexOf("&"));
                }
            }
            Console.WriteLine(httpUrl.Substring(1));
            if (File.Exists(WebUtility.UrlDecode(httpUrl.Substring(1))) && Settings.Default.Dlna) {
                DlnaRequest(httpUrl, processor);
            } else {
                string result = string.Empty;
                if (httpUrl.IndexOf("/treeview") == 0 && Settings.Default.Dlna) {
                    result = TreeviewRequest(text, processor);
                } else {
                    if (httpUrl.IndexOf("/parserlink") == 0) {
                        result = ParserlinkRequest(httpUrl);
                    } else {
                        if (httpUrl.IndexOf("/test") == 0) {
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
