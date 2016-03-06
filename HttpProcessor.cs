using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace RemoteFork {
    public class HttpProcessor {
        public TcpClient socket;
        public HttpServer srv;
        private Stream inputStream;
        public StreamWriter outputStream;
        public string http_method;
        public string http_url;
        public string http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        private static int MAX_POST_SIZE = 10485760;
        private const int BUF_SIZE = 4096;

        public HttpProcessor(TcpClient s, HttpServer srv) {
            socket = s;
            this.srv = srv;
        }

        public void Process() {
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
            string text = Tools.StreamReadLine(inputStream);
            string[] array = text.Split(' ');
            bool flag = array.Length != 3;
            if (flag) {
                throw new Exception("invalid http request line");
            }
            http_method = array[0].ToUpper();
            http_url = array[1];
            http_protocol_versionstring = array[2];
            Console.WriteLine("starting: " + text);
        }

        public void ReadHeaders() {
            string text;
            while ((text = Tools.StreamReadLine(inputStream)) != null) {
                bool flag = text.Equals("");
                if (flag) {
                    break;
                }
                int num = text.IndexOf(':');
                bool flag2 = num == -1;
                if (flag2) {
                    throw new Exception("invalid http header line: " + text);
                }
                string key = text.Substring(0, num);
                int num2 = num + 1;
                while (num2 < text.Length && text[num2] == ' ') {
                    num2++;
                }
                string value = text.Substring(num2, text.Length - num2);
                httpHeaders[key] = value;
            }
        }

        public void HandleGetRequest() {
            srv.HandleGetRequest(this);
        }

        public void HandlePostRequest() {
            Console.WriteLine("get post data start");
            MemoryStream memoryStream = new MemoryStream();
            bool flag = httpHeaders.ContainsKey("Content-Length");
            if (flag) {
                int num = Convert.ToInt32(httpHeaders["Content-Length"]);
                bool flag2 = num > MAX_POST_SIZE;
                if (flag2) {
                    throw new Exception(string.Format("POST Content-Length({0}) too big for this simple server", num));
                }
                byte[] buffer = new byte[4096];
                int i = num;
                while (i > 0) {
                    Console.WriteLine("starting Read, to_read={0}", i);
                    int num2 = inputStream.Read(buffer, 0, Math.Min(4096, i));
                    Console.WriteLine("read finished, numread={0}", num2);
                    bool flag3 = num2 == 0;
                    if (flag3) {
                        bool flag4 = i == 0;
                        if (flag4) {
                            break;
                        }
                        throw new Exception("client disconnected during post");
                    } else {
                        i -= num2;
                        memoryStream.Write(buffer, 0, num2);
                    }
                }
                memoryStream.Seek(0L, SeekOrigin.Begin);
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
            } catch (Exception value) {
                Console.WriteLine(value);
            }
        }
    }
}
