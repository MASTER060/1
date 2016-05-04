using System;
using System.Collections.Generic;
using System.IO;
using RemoteFork.Server;

namespace RemoteFork.Requestes {
    internal class DlnaFileRequest : ProcessorRequest {
        public DlnaFileRequest(string text, HttpProcessor processor) : base(text, processor) {
        }

        public override string Execute() {
            Console.WriteLine("dlna file");
            using (
                var fileStream = new FileStream(System.Web.HttpUtility.UrlDecode(text),
                    FileMode.Open, FileAccess.Read, FileShare.Read)) {
                try {
                    List<string> content = new List<string>();

                    long count = fileStream.Length;
                    long offset = 0;
                    if (!string.IsNullOrEmpty(processor.Range)) {
                        string[] range = processor.Range.Split('-');
                        offset = long.Parse(range[0]);
                        if (!string.IsNullOrWhiteSpace(range[1])) {
                            long.TryParse(range[1], out count);
                        }
                    }

                    processor.SetAutoFlush(false);
                    long lenght = count - offset;
                    string type = Tools.GetMimeType(text);
                    if (type.Contains("text") || type.Contains("image") || string.IsNullOrEmpty(processor.Range)) {
                        content.AddRange(new[] {
                            "HTTP/1.1 200 OK",
                            "Access-Control-Allow-Origin: *",
                            string.Format("Content-Length: {0}", lenght),
                        });
                    } else {
                        content.AddRange(new[] {
                            "HTTP/1.1 206 Partial Content",
                            "Accept-Ranges: bytes",
                            string.Format("Content-Range: bytes {0}-{1}/{2}", offset, offset + lenght - 1, count),
                            string.Format("Content-Length: {0}", lenght),
                        });
                    }
                    content.Add(string.Format("Content-Type: {0}", type));
                    content.Add(string.Empty);
                    processor.WriteLines(content.ToArray());
                    processor.SetAutoFlush(true);
                    Console.WriteLine("fs.Seek=" + offset);
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    Console.WriteLine("starting Read, to_read={0}", lenght);
                    while (lenght > 0L) {
                        byte[] buffer = new byte[256000];
                        int num6 = fileStream.Read(buffer, 0, (int) Math.Min(256000, lenght));
                        if (num6 == 0) {
                            break;
                        }
                        lenght -= num6;
                        processor.WriteBaseStream(buffer, 0, num6);
                    }
                } finally {
                    Console.WriteLine("fs read end ");
                    fileStream.Close();
                }
            }

            return string.Empty;
        }
    }
}
