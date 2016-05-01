using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using RemoteFork.Properties;
using RemoteFork.Requestes;

namespace RemoteFork.Server {
    internal class MyHttpServer : HttpServer {
        public MyHttpServer(IPAddress ip, int port) : base(ip, port) {
        }

        public override async Task HandleGetRequest(HttpProcessor processor) {
            string httpUrl = System.Web.HttpUtility.UrlDecode(processor.HttpUrl);
            string result = string.Empty;

            if (!string.IsNullOrEmpty(httpUrl)) {
                Console.WriteLine(httpUrl.Substring(1));
                if (File.Exists(httpUrl.Substring(1)) && Settings.Default.Dlna) {

                    if (Settings.Default.DlnaFilterType == 2) {
                        if (Settings.Default.DlnaDirectories != null) {
                            var filter = new List<string>(Settings.Default.DlnaDirectories.Cast<string>());
                            if (filter.Any(i => httpUrl.Contains(i))) {
                                Console.WriteLine("request: {0}", processor.HttpUrl);
                                processor.WriteFailure();
                                return;
                            }
                        }
                    }
                    BaseRequest request = new DlnaFileRequest(httpUrl, processor);
                    await request.Execute();
                } else {
                    BaseRequest request = null;

                    if (httpUrl.StartsWith("/treeview?plugin")) {
                        request = new PluginRequest(httpUrl, processor);
                    } else if (httpUrl.StartsWith("/treeview") && Settings.Default.Dlna) {
                        string text = string.Empty;
                        if (httpUrl.Length > 10) {
                            text = httpUrl.Substring(10);
                            if (httpUrl.IndexOf("&", StringComparison.Ordinal) > 0) {
                                text = text.Substring(0, text.IndexOf("&", StringComparison.Ordinal));
                            }
                        }
                        request = new DlnaBrowserRequest(text, processor);
                    } else {
                        if (httpUrl.StartsWith("/parserlink")) {
                            request = new ParseLinkRequest(httpUrl);
                        } else {
                            if (httpUrl.StartsWith("/test")) {
                                request = new TestRequest(httpUrl);
                            }
                        }
                    }
                    if (request != null) {
                        result = await request.Execute();
                    }

                    Console.WriteLine("request: {0}", processor.HttpUrl);
                    processor.WriteSuccess();
                    processor.WriteLine(result);
                }
            } else {
                Console.WriteLine("request: {0}", processor.HttpUrl);
                processor.WriteFailure();
            }
        }

        public override async Task HandlePostRequest(HttpProcessor processor, StreamReader inputData) {
            Console.WriteLine("POST request: {0}", processor.HttpUrl);
            string arg = await inputData.ReadToEndAsync();
            processor.WriteSuccess();
            processor.WriteLines("<html><body><h1>test server</h1>",
                "<a href=/test>return</a><p>",
                string.Format("postbody: <pre>{0}</pre>", arg));
        }
    }
}
