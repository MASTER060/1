using System;
using System.IO;
using System.Threading.Tasks;
using RemoteFork.Server;

namespace RemoteFork.Requestes {
    internal class DlnaFileRequest : ProcessorRequest {
        public DlnaFileRequest(string text, HttpProcessor processor) : base(text, processor) {
        }

        public override async Task<string> Execute() {
            Console.WriteLine("dlna file");
            using (
                var fileStream = new FileStream(System.Web.HttpUtility.UrlDecode(text.Substring(1)),
                    FileMode.Open, FileAccess.Read, FileShare.Read)) {
                try {
                    long count = fileStream.Length;
                    long offset = 0;
                    if (!string.IsNullOrEmpty(processor.Range)) {
                        string[] range = processor.Range.Split('-');
                        offset = long.Parse(range[0]);
                        if (!string.IsNullOrWhiteSpace(range[1])) {
                            long.TryParse(range[1], out count);
                        }
                    }

                    processor.SetAutoFlush(true);
                    long lenght = count - offset;
                    string[] content = new[] {
                        "HTTP/1.0 206 Partial Content",
                        "Content-Type: video/mp4",
                        "Accept-Ranges: bytes",
                        string.Format("Content-Range: bytes {0}-{1}/{2}", offset, fileStream.Length - 1L,
                            fileStream.Length),
                        "Content-Length: " + lenght,
                        "Connection: Close",
                        ""
                    };
                    processor.WriteLines(content);
                    processor.SetAutoFlush(true);
                    long size = 256000L;
                    Console.WriteLine("fs.Seek=" + offset);
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    Console.WriteLine("starting Read, to_read={0}", lenght);
                    while (lenght > 0L) {
                        byte[] buffer = new byte[size];
                        int readCount = fileStream.Read(buffer, 0, (int)Math.Min(size, lenght));
                        if (count == 0) {
                            break;
                        }
                        lenght -= count;
                        processor.WriteBaseStream(buffer, 0, readCount);
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
