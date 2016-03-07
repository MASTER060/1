using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RemoteFork {
    public class HttpProcessor {
        public TcpClient socket;
        public HttpServer srv;
        public StreamWriter outputStream;
        public string http_method;
        public string http_url;
        public string http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        private Stream inputStream;
        private static int MAX_POST_SIZE = 10485760;
        private const int BUF_SIZE = 4096;

        public HttpProcessor(TcpClient client, HttpServer srv) {
            socket = client;
            this.srv = srv;
        }

        public void Process(object state) {
            inputStream = new BufferedStream(socket.GetStream());
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try {
                ParseRequest();
                ReadHeaders();
                bool flag = http_method.Equals("GET");
                if (flag) {
                    HandleGetRequest();
                } else {
                    bool flag2 = http_method.Equals("POST");
                    if (flag2) {
                        HandlePostRequest();
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine("Exception: " + ex);
                WriteFailure();
            }
            try {
                outputStream.Flush();
            } catch (Exception ex2) {
                Console.WriteLine("Exception: " + ex2);
            }
            inputStream = null;
            outputStream = null;
            Console.WriteLine("Socked close");
            socket.Close();
        }

        public void ParseRequest() {
            string text = StreamReadLine(inputStream);
            string[] array = text.Split(' ');
            if (array.Length == 3) {
                http_method = array[0].ToUpper();
                http_url = array[1];
                http_protocol_versionstring = array[2];
                Console.WriteLine("starting: " + text);
            } else {
                throw new Exception("invalid http request line");
            }
        }

        public void ReadHeaders() {
            string text;
            while ((text = StreamReadLine(inputStream)) != null) {
                if (!string.IsNullOrEmpty(text)) {
                    int num = text.IndexOf(':');
                    if (num != -1) {
                        string key = text.Substring(0, num);
                        int num2 = num + 1;
                        while (num2 < text.Length && text[num2] == ' ') {
                            num2++;
                        }
                        string value = text.Substring(num2, text.Length - num2);
                        httpHeaders[key] = value;
                    } else {
                        throw new Exception("invalid http header line: " + text);
                    }
                } else {
                    break;
                }
            }
        }

        public void HandleGetRequest() {
            srv.HandleGetRequest(this);
        }

        public void HandlePostRequest() {
            Console.WriteLine("get post data start");
            MemoryStream memoryStream = new MemoryStream();
            if (httpHeaders.ContainsKey("Content-Length")) {
                int num = Convert.ToInt32(httpHeaders["Content-Length"]);
                if (num <= MAX_POST_SIZE) {
                    byte[] buffer = new byte[BUF_SIZE];
                    int i = num;
                    while (i > 0) {
                        Console.WriteLine("starting Read, to_read={0}", i);
                        int num2 = inputStream.Read(buffer, 0, Math.Min(BUF_SIZE, i));
                        Console.WriteLine("read finished, numread={0}", num2);
                        if (num2 == 0) {
                            if (i != 0) {
                                throw new Exception("client disconnected during post");
                            }
                        } else {
                            i -= num2;
                            memoryStream.Write(buffer, 0, num2);
                        }
                    }
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                } else {
                    throw new Exception(string.Format("POST Content-Length({0}) too big for this simple server", num));
                }
            }
            Console.WriteLine("get post data end");
            srv.HandlePostRequest(this, new StreamReader(memoryStream));
        }

        public void WriteSuccess(string contentType = "text/html") {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Access-Control-Allow-Origin: *");
            outputStream.WriteLine("Content-Type: text/html; charset=UTF-8");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        public void WriteFailure() {
            try {
                outputStream.WriteLine("HTTP/1.0 404 File not found");
                outputStream.WriteLine("Connection: close");
                outputStream.WriteLine("");
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private static string StreamReadLine(Stream inputStream) {
            string text = string.Empty;
            while (true) {
                int num = inputStream.ReadByte();
                if (num != 10) {
                    if (num != 13) {
                        if (num != -1) {
                            text += Convert.ToChar(num).ToString();
                        } else {
                            Thread.Sleep(1);
                        }
                    }
                } else {
                    break;
                }
            }
            return text;
        }
    }
}
