using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using NLog;
using RemoteFork.Network;
using RemoteFork.Properties;
using Unosquare.Labs.EmbedIO;
using HttpListenerContext = Unosquare.Net.HttpListenerContext;
using HttpListenerResponse = Unosquare.Net.HttpListenerResponse;

namespace RemoteFork.Requestes {
    internal class DlnaFileRequestHandler : BaseRequestHandler {
        private static readonly ILogger Log =
            LogManager.GetLogger("DlnaFileRequestHandler", typeof(DlnaFileRequestHandler));

        public override void Handle(HttpListenerContext context) {
            Console.WriteLine("Handle get file");
            string file = context.Request.QueryString.GetValues(string.Empty)
                ?.FirstOrDefault(s => s.StartsWith(Uri.UriSchemeFile));
            if (!string.IsNullOrEmpty(file)) {
                try {
                    var fileRequest = FileRequest.Create(context, file);

                    if (fileRequest.File.Exists && Tools.CheckAccessPath(fileRequest.File.FullName)) {
                        string requestId = DateTime.Now.Ticks.ToString("x2");

                        Log.Debug(
                            "[{0}] Requested: {1}, Rnages: [{2}]-[{3}]/{4}",
                            requestId,
                            fileRequest.File.FullName,
                            string.Join(",", fileRequest.RangesStartIndexes),
                            string.Join(",", fileRequest.RangesEndIndexes),
                            fileRequest.File.Length
                        );

                        context.Response.AddHeader("Accept-Ranges", "bytes");
                        if (ValidateRanges(fileRequest, context.Response) &&
                            ValidateModificationDate(fileRequest,
                                context) && ValidateEntityTag(fileRequest, context)) {
                            context.Response.AddHeader("Last-Modified", fileRequest.File.LastWriteTime.ToString("r"));
                            context.Response.AddHeader("Etag", fileRequest.EntityTag);

                            if (!fileRequest.RangeRequest) {
                                context.Response.ContentLength64 = fileRequest.File.Length;
                                context.Response.ContentType = fileRequest.ContentType;
                                context.Response.StatusCode = (int) HttpStatusCode.OK;

                                if (context.RequestVerb() != HttpVerbs.Head) {
                                    using (var fileStream = fileRequest.File.OpenRead()) {
                                        fileStream.CopyTo(context.Response.OutputStream,
                                            Settings.Default.FileBufferSize);

                                        //long sent = Copy(fileStream, context.Response.OutputStream,
                                        //    Settings.Default.FileBufferSize);
                                        //Log.Debug("[{0}] Sent: {1} bytes", requestId, sent);
                                    }
                                }
                            } else {
                                context.Response.ContentLength64 = (int) fileRequest.GetContentLength();

                                if (!fileRequest.MultipartRequest) {
                                    context.Response.AddHeader("Content-Range",
                                        $"bytes {fileRequest.RangesStartIndexes[0]}-{fileRequest.RangesEndIndexes[0]}/{fileRequest.File.Length}");
                                    context.Response.ContentType = fileRequest.ContentType;
                                } else {
                                    context.Response.ContentType =
                                        $"multipart/byteranges; boundary={fileRequest.Boundary}";
                                }

                                context.Response.StatusCode = (int) HttpStatusCode.PartialContent;
                                if (context.RequestVerb() != HttpVerbs.Head) {
                                    using (var fileStream = fileRequest.File.OpenRead()) {
                                        for (int i = 0; i < fileRequest.RangesStartIndexes.Length; i++) {
                                            if (fileRequest.MultipartRequest) {
                                                Copy($"--{fileRequest.Boundary}\r\n", context.Response.OutputStream);
                                                Copy($"Content-Type: {fileRequest.ContentType}\r\n",
                                                    context.Response.OutputStream);
                                                Copy(
                                                    $"Content-Range: bytes {fileRequest.RangesStartIndexes[i]}-{fileRequest.RangesEndIndexes[i]}/{fileRequest.File.Length}\r\n\r\n",
                                                    context.Response.OutputStream);
                                            }

                                            Copy(
                                                fileStream,
                                                context.Response.OutputStream,
                                                fileRequest.RangesStartIndexes[i],
                                                fileRequest.RangesEndIndexes[i] - fileRequest.RangesStartIndexes[i] + 1,
                                                Settings.Default.FileBufferSize
                                            );

                                            if (fileRequest.MultipartRequest) {
                                                Copy("\r\n", context.Response.OutputStream);
                                            }
                                        }

                                        if (fileRequest.MultipartRequest) {
                                            Copy($"--{fileRequest.Boundary}--", context.Response.OutputStream);
                                        }
                                    }
                                }
                            }
                        }
                    } else {
                        Log.Debug("File not found: {0}", file);

                        HTTPUtility.WriteResponse(context.Response, HttpStatusCode.NotFound, $"File not found: {file}");
                    }
                } catch (Exception exception) {
                    Log.Error(exception);
                }
            } else {
                Log.Debug("Incorrect parameter: {0}", file);

                HTTPUtility.WriteResponse(context.Response, HttpStatusCode.NotFound, $"Incorrect parameter: {file}");
            }
        }

        //private static long Copy(Stream input, Stream output, int bufferSize) {
        //    long totalRead = 0;
        //    var buffer = new byte[bufferSize];

        //    int bytesRead;
        //    while ((bytesRead = input.Read(buffer, 0, bufferSize)) != 0) {
        //        output.Write(buffer, 0, bytesRead);
        //        totalRead += bytesRead;
        //        output.Flush();
        //    }

        //    return totalRead;
        //}

        public void Copy(Stream input, Stream output, long inputOffset, long length, int bufferSize) {
            long totalRead = 0;

            if (length > 0) {
                var buffer = new byte[bufferSize];

                if (inputOffset > 0) {
                    input.Seek(inputOffset, SeekOrigin.Begin);
                }

                int bufferLength = buffer.Length;
                int bytesToRead = bufferLength;

                if (length < bufferLength) {
                    bytesToRead = Convert.ToInt32(length);
                }

                int read;
                while (bytesToRead > 0 && 0 != (read = input.Read(buffer, 0, bytesToRead))) {
                    output.Write(buffer, 0, read);

                    totalRead += read;

                    bytesToRead = Convert.ToInt32(Math.Min(length - totalRead, bufferLength));

                    output.Flush();
                }
            }
        }

        private static void Copy(string input, Stream output) {
            if (input != null) {
                var buffer = Encoding.UTF8.GetBytes(input);
                output.Write(buffer, 0, buffer.Length);
            }
        }

        private static bool ValidateRanges(FileRequest fileRequest, HttpListenerResponse response) {
            long fileLength = fileRequest.File.Length;

            for (int i = 0; i < fileRequest.RangesStartIndexes.Length; i++) {
                if ((fileRequest.RangesStartIndexes[i] > fileLength - 1)
                    || (fileRequest.RangesEndIndexes[i] > fileLength - 1)
                    || (fileRequest.RangesStartIndexes[i] < 0)
                    || (fileRequest.RangesEndIndexes[i] < 0)
                    || (fileRequest.RangesEndIndexes[i] < fileRequest.RangesStartIndexes[i])) {
                    response.StatusCode = (int) HttpStatusCode.NotAcceptable;
                    response.AddHeader("Content-Range", $"bytes */{fileLength}");

                    return false;
                }
            }

            return true;
        }

        private static bool ValidateModificationDate(FileRequest fileRequest, HttpListenerContext context) {
            if (context.Request.Headers.AllKeys.Contains("If-Modified-Since")) {
                string modifiedSinceHeader = context.Request.Headers["If-Modified-Since"];
                if (!string.IsNullOrEmpty(modifiedSinceHeader)) {
                    var modifiedSinceDate = FileRequest.ParseHttpDateHeader(context, "If-Modified-Since");

                    if (fileRequest.File.LastWriteTime <= modifiedSinceDate) {
                        context.Response.StatusCode = (int) HttpStatusCode.NotModified;
                        return false;
                    }
                }
            }

            if (context.Request.Headers.AllKeys.Contains("If-Unmodified-Since")) {
                string unmodifiedSinceHeader = context.Request.Headers["If-Unmodified-Since"];

                if (string.IsNullOrEmpty(unmodifiedSinceHeader)) {
                    if (context.Request.Headers.AllKeys.Contains("Unless-Modified-Since")) {
                        unmodifiedSinceHeader = context.Request.Headers["Unless-Modified-Since"];
                    }
                }

                if (!string.IsNullOrEmpty(unmodifiedSinceHeader)) {
                    var unmodifiedSinceDate = FileRequest.ParseHttpDateHeader(context, "If-Modified-Since");

                    if (fileRequest.File.LastWriteTime > unmodifiedSinceDate) {
                        context.Response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ValidateEntityTag(FileRequest fileRequest, HttpListenerContext context) {
            if (context.Request.Headers.AllKeys.Contains("If-Match")) {
                string matchHeader = context.Request.Headers["If-Match"];
                if (matchHeader != "*") {
                    var entitiesTags = matchHeader.Split(FileRequest.CommaSplitArray);
                    int entitieTagIndex = 0;

                    for (; entitieTagIndex < entitiesTags.Length; entitieTagIndex++) {
                        if (fileRequest.EntityTag == entitiesTags[entitieTagIndex]) {
                            break;
                        }
                    }

                    if (entitieTagIndex >= entitiesTags.Length) {
                        context.Response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                        return false;
                    }
                }
            }

            if (context.Request.Headers.AllKeys.Contains("If-None-Match")) {
                string noneMatchHeader = context.Request.Headers["If-None-Match"];
                if (!string.IsNullOrEmpty(noneMatchHeader)) {
                    if (noneMatchHeader == "*") {
                        context.Response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                        return false;
                    }

                    var entitiesTags = noneMatchHeader.Split(FileRequest.CommaSplitArray);
                    if (entitiesTags.Any(entityTag => fileRequest.EntityTag == entityTag)) {
                        context.Response.AddHeader("ETag", fileRequest.EntityTag);
                        context.Response.StatusCode = (int) HttpStatusCode.NotModified;
                        return false;
                    }
                }
            }

            return true;
        }
    }

    internal sealed class FileRequest {
        private static readonly MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();

        //private static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>();

        private const string HeaderIfRange = "If-Range";

        private static readonly string[] HttpDateFormats =
            {"r", "dddd, dd-MMM-yy HH':'mm':'ss 'GMT'", "ddd MMM d HH':'mm':'ss yyyy"};

        internal static readonly char[] CommaSplitArray = {','};

        private static readonly char[] DashSplitArray = {'-'};

        public string Boundary { get; } = "---------------------------" + DateTime.Now.Ticks.ToString("x");

        private readonly HttpListenerContext _context;

        private FileRequest(HttpListenerContext context, string file) {
            _context = context;
            File = new FileInfo(new Uri(file).LocalPath);
        }

        public FileInfo File { get; }

        public bool RangeRequest { get; private set; }

        public bool MultipartRequest { get; private set; }

        public string EntityTag { get; private set; }

        public string ContentType { get; private set; }

        public long[] RangesStartIndexes { get; private set; }
        public long[] RangesEndIndexes { get; private set; }

        private void Parse() {
            EntityTag = GenerateEntityTag();
            ContentType = Constants.DefaultMimeTypes.ContainsKey(File.Extension)
                ? Constants.DefaultMimeTypes[File.Extension]
                : "application/octet-stream";
            ParseRanges();
        }

        private void ParseRanges() {
            if (_context.Request.Headers.AllKeys.Contains("Range")) {
                string rangesHeader = _context.Request.Headers["Range"];
                string ifRangeHeader = _context.Request.Headers[HeaderIfRange];
                var ifRangeHeaderDate = ParseHttpDateHeader(_context, HeaderIfRange);

                if (string.IsNullOrEmpty(rangesHeader)
                    || (!string.IsNullOrEmpty(ifRangeHeader)
                        && (ifRangeHeader != EntityTag
                            || (ifRangeHeaderDate != DateTime.MinValue && File.LastWriteTime > ifRangeHeaderDate)))) {
                    RangesStartIndexes = new[] {0L};
                    RangesEndIndexes = new[] {File.Length - 1};
                    RangeRequest = false;
                    MultipartRequest = false;
                } else {
                    var ranges = rangesHeader.Replace("bytes=", string.Empty).Split(CommaSplitArray);

                    RangesStartIndexes = new long[ranges.Length];
                    RangesEndIndexes = new long[ranges.Length];
                    RangeRequest = true;
                    MultipartRequest = ranges.Length > 1;

                    for (var i = 0; i < ranges.Length; i++) {
                        var currentRange = ranges[i].Split(DashSplitArray);

                        if (string.IsNullOrEmpty(currentRange[1])) {
                            RangesEndIndexes[i] = File.Length - 1;
                        } else {
                            RangesEndIndexes[i] = long.Parse(currentRange[1]);
                        }

                        if (string.IsNullOrEmpty(currentRange[0])) {
                            RangesStartIndexes[i] = File.Length - 1 - RangesEndIndexes[i];
                            RangesEndIndexes[i] = File.Length - 1;
                        } else {
                            RangesStartIndexes[i] = long.Parse(currentRange[0]);
                        }
                    }
                }
            } else {
                RangesStartIndexes = new[] {0L};
                RangesEndIndexes = new[] {(File.Length - 1)};
                RangeRequest = false;
                MultipartRequest = false;
            }
        }

        public long GetContentLength() {
            long contentLength = 0L;

            for (int i = 0; i < RangesStartIndexes.Length; i++) {
                contentLength += Convert.ToInt64(RangesEndIndexes[i] - RangesStartIndexes[i]) + 1;

                if (MultipartRequest) {
                    contentLength +=
                        Boundary.Length
                        + ContentType.Length
                        + RangesStartIndexes[i].ToString().Length
                        + RangesEndIndexes[i].ToString().Length
                        + File.Length.ToString().Length
                        + 49;
                }
            }

            if (MultipartRequest) {
                contentLength += Boundary.Length + 4;
            }
            return contentLength;
        }

        internal static DateTime ParseHttpDateHeader(HttpListenerContext context, string headerName) {
            if (context.Request.Headers.AllKeys.Contains("headerName")) {
                string ifRangeHeader = context.Request.Headers[headerName];
                if (DateTime.TryParseExact(ifRangeHeader, HttpDateFormats, null,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out DateTime ifRangeHeaderDate))
                    return ifRangeHeaderDate;
            }

            return DateTime.MinValue;
        }

        private string GenerateEntityTag() {
            return Convert.ToBase64String(
                Md5.ComputeHash(Encoding.ASCII.GetBytes($"{File.FullName}|{File.LastWriteTime}")));
        }

        public static FileRequest Create(HttpListenerContext context, string file) {
            var fr = new FileRequest(context, file);

            fr.Parse();

            return fr;
        }
    }
}
