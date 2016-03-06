using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace RemoteFork {
    public class MyHttpServer : HttpServer {
        public string[] sets;

        public void SaveSets() {
            string text = "";
            for (int i = 0; i < sets.Length; i++) {
                bool flag = i > 0;
                if (flag) {
                    text += "|";
                }
                text += sets[i];
            }
            Console.WriteLine(Environment.CurrentDirectory + "\\main.cfg");
            File.WriteAllText(Environment.CurrentDirectory + "\\main.cfg", text);
        }

        public string[] GetSets() {
            string text = "";
            bool flag = File.Exists(Environment.CurrentDirectory + "\\main.cfg");
            if (flag) {
                try {
                    using (StreamReader streamReader = new StreamReader(Environment.CurrentDirectory + "\\main.cfg")
                        ) {
                        text = streamReader.ReadToEnd();
                    }
                } catch (Exception) {
                    text = "";
                }
            }
            bool flag2 = text == "";
            if (flag2) {
                text = "ServerAutoStart|GetIpAuto|||";
            }
            return text.Split('|');
        }

        public MyHttpServer(IPAddress ip, int port) : base(ip, port) {
        }

        public override void HandleGetRequest(HttpProcessor p) {
            sets = GetSets();
            string httpUrl = p.http_url;
            string text = "";
            bool flag = httpUrl.Length > 10;
            if (flag) {
                text = WebUtility.UrlDecode(httpUrl.Substring(10));
                bool flag2 = httpUrl.IndexOf("&") > 0;
                if (flag2) {
                    text = text.Substring(0, text.IndexOf("&"));
                }
            }
            Console.WriteLine(httpUrl.Substring(1));
            bool flag3 = File.Exists(WebUtility.UrlDecode(httpUrl.Substring(1))) && sets[4] == "";
            if (flag3) {
                Console.WriteLine("video");
                using (
                    FileStream fileStream = new FileStream(WebUtility.UrlDecode(httpUrl.Substring(1)),
                        FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    try {
                        long num = -1L;
                        bool flag4 = p.httpHeaders.Contains("Range");
                        long num2;
                        if (flag4) {
                            string text2 = p.httpHeaders["Range"].ToString().Replace("bytes=", "");
                            string[] array = text2.Split('-');
                            num2 = long.Parse(array[0]);
                            bool flag5 = array[1].Trim().Length > 0;
                            if (flag5) {
                                long.TryParse(array[1], out num);
                            }
                            bool flag6 = num == -1L;
                            if (flag6) {
                                num = fileStream.Length;
                            }
                        } else {
                            num2 = 0L;
                            num = fileStream.Length;
                        }
                        p.outputStream.AutoFlush = true;
                        p.outputStream.WriteLine("HTTP/1.0 206 Partial Content");
                        p.outputStream.WriteLine("Content-Type: video/mp4");
                        p.outputStream.WriteLine("Accept-Ranges: bytes");
                        long num3 = num - num2;
                        p.outputStream.WriteLine("Content-Range: bytes {0}-{1}/{2}", num2, fileStream.Length - 1L,
                            fileStream.Length);
                        p.outputStream.WriteLine("Content-Length: " + (num - num2));
                        p.outputStream.WriteLine("Connection: Close");
                        p.outputStream.WriteLine("");
                        p.outputStream.AutoFlush = true;
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
                            bool flag9 = num6 == 0;
                            if (flag9) {
                                bool flag10 = num5 == 0L;
                                if (flag10) {
                                    break;
                                }
                                break;
                            } else {
                                num5 -= (long) num6;
                                p.outputStream.BaseStream.Write(buffer, 0, num6);
                            }
                        }
                    } finally {
                        Console.WriteLine("fs read end ");
                        fileStream.Close();
                    }
                }
            } else {
                string text3 = "";
                bool flag11 = httpUrl.IndexOf("/treeview") == 0 && sets[4] == "";
                if (flag11) {
                    string text4 = "http://" + p.httpHeaders["Host"] + "/";
                    text3 = "#EXTM3U\\n";
                    bool flag12 = text == "/";
                    if (flag12) {
                        DriveInfo[] drives = DriveInfo.GetDrives();
                        for (int i = 0; i < drives.Length; i++) {
                            bool isReady = drives[i].IsReady;
                            if (isReady) {
                                string text5 = string.Concat(drives[i].Name, " (",
                                    Tools.FSize(drives[i].AvailableFreeSpace), " свободно из ",
                                    Tools.FSize(drives[i].TotalSize), ")");
                                string text6 = string.Concat(text5, "<br>Метка диска: ", drives[i].VolumeLabel,
                                    "<br>Тип носителя: ", drives[i].DriveType);
                                text3 = string.Concat(text3, "#EXTINF:-1,", text6, "\n", text4, "treeview?",
                                    drives[i].Name, "\n");
                                Console.WriteLine(text6);
                            }
                        }
                    } else {
                        string[] array2 = Directory.GetDirectories(text);
                        for (int j = 0; j < array2.Length; j++) {
                            string text7 = Uri.EscapeUriString(array2[j] + "\\");
                            text3 = string.Concat(text3, "#EXTINF:-1,", array2[j].Split('\\').Last<string>(), "\n",
                                text4, "treeview?", text7, "\n");
                            Console.WriteLine(array2[j]);
                        }
                        array2 = Directory.GetFiles(text);
                        for (int k = 0; k < array2.Length; k++) {
                            string text8 = Uri.EscapeUriString(array2[k]);
                            FileInfo fileInfo = new FileInfo(array2[k]);
                            text3 = string.Concat(text3, "#EXTINF:-1,", array2[k].Split('\\').Last<string>(), " ",
                                Tools.FSize(fileInfo.Length), "\n", text4, text8, "\n");
                            Console.WriteLine(array2[k]);
                        }
                    }
                } else {
                    bool flag13 = httpUrl.IndexOf("/parserlink") == 0;
                    if (flag13) {
                        string text9 = WebUtility.UrlDecode(httpUrl.Substring(12));
                        string[] array3 = text9.Split('|');
                        Console.WriteLine("parse0 " + array3[0]);
                        bool flag14 = array3[0].IndexOf("curl") == 0;
                        string text10;
                        if (flag14) {
                            text10 = Environment.CurrentDirectory + "\\" + array3[0];
                        } else {
                            text10 = Environment.CurrentDirectory + "\\curl \"" + array3[0].Replace("\"", "\"\"") +
                                     "\"";
                            text10 = text10 + " -H \"User-Agent:" +
                                     p.httpHeaders["User-Agent"].ToString().Replace("\"", "\"\"") + "\"";
                        }
                        /*Console.WriteLine("cmd " + text10);
                        Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/C " + text10) {
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });
                        StreamReader standardOutput = process.StandardOutput;
                        string text11 = standardOutput.ReadToEnd();
                        Console.WriteLine(text11);
                        process.WaitForExit();*/
                        
                        string text11 =
                            HttpUtility.GetRequest(new Uri(array3[0].Replace("\"", "\"\""))).Result;

                        bool flag15 = array3.Length == 1;
                        if (flag15) {
                            text3 = text11;
                        } else {
                            bool flag16 = array3[1].IndexOf(".*?") == -1;
                            if (flag16) {
                                bool flag17 = array3[1] == "" && array3[2] == "";
                                if (flag17) {
                                    text3 = text11;
                                } else {
                                    int num7 = text11.IndexOf(array3[1]);
                                    bool flag18 = num7 == -1;
                                    if (flag18) {
                                        text3 = "";
                                    } else {
                                        num7 += array3[1].Length;
                                        int num8 = text11.IndexOf(array3[2], num7);
                                        bool flag19 = num8 == -1;
                                        if (flag19) {
                                            text3 = "";
                                        } else {
                                            text3 = text11.Substring(num7, num8 - num7);
                                        }
                                    }
                                }
                            } else {
                                Console.WriteLine(array3[1] + "(.*?)" + array3[2]);
                                string pattern = array3[1] + "(.*?)" + array3[2];
                                Regex regex = new Regex(pattern, RegexOptions.Multiline);
                                Match match = regex.Match(text11);
                                bool success = match.Success;
                                if (success) {
                                    text3 = match.Groups[1].Captures[0].ToString();
                                }
                            }
                        }
                    } else {
                        bool flag20 = httpUrl.IndexOf("/test") == 0;
                        if (flag20) {
                            text3 =
                                "<html><h1>ForkPlayer DLNA Work!</h1>";
                            bool flag21 = httpUrl.IndexOf('|') > 0;
                            if (flag21) {
                                string text12 = httpUrl.Substring(6);
                                sets = GetSets();
                                sets[3] = text12.Replace('|', '~');
                                SaveSets();
                            }
                        }
                    }
                }
                Console.WriteLine("request: {0}", p.http_url);
                p.WriteSuccess("text/html");
                p.outputStream.WriteLine(text3);
                Console.WriteLine("List end");
            }
        }

        public override void HandlePostRequest(HttpProcessor p, StreamReader inputData) {
            Console.WriteLine("POST request: {0}", p.http_url);
            string arg = inputData.ReadToEnd();
            p.WriteSuccess("text/html");
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", arg);
        }
    }
}
