using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RemoteFork.Requestes {
    internal class DlnaFileRequestHandler : BaseRequestHandler {
        internal static readonly string UrlPath = "/file";
        private static readonly ILogger Log = Program.LoggerFactory.CreateLogger<DlnaFileRequestHandler>();

        public Stream HandleStream(HttpContext context) {
            Log.LogDebug("HandleStream get file");

            string file = string.Empty;

            if (context.Request.Query.ContainsKey(string.Empty)) {
                file = context.Request.Query[string.Empty].FirstOrDefault(s => s.StartsWith(Uri.UriSchemeFile));
            }

            if (!string.IsNullOrEmpty(file)) {
                try {
                    var fileRequest = FileRequest.Create(context, file);

                    if (fileRequest.File.Exists && Tools.CheckAccessPath(fileRequest.File.FullName)) {

                        context.Response.Headers.Add("Accept-Ranges", "bytes");
                        if (ValidateRanges(fileRequest, context.Response) &&
                            ValidateModificationDate(fileRequest,
                                context) && ValidateEntityTag(fileRequest, context)) {
                            context.Response.Headers.Add("Last-Modified", fileRequest.File.LastWriteTime.ToString("r"));
                            context.Response.Headers.Add("Etag", fileRequest.EntityTag);

                            if (!fileRequest.RangeRequest) {
                                context.Response.ContentLength = fileRequest.File.Length;
                                context.Response.ContentType = fileRequest.ContentType;
                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                                
                                var fileStream = fileRequest.File.OpenRead();
                                return fileStream;
                            } else {
                                context.Response.ContentLength = fileRequest.GetContentLength();

                                if (!fileRequest.MultipartRequest) {
                                    context.Response.Headers.Add("Content-Range",
                                        $"bytes {fileRequest.RangesStartIndexes[0]}-{fileRequest.RangesEndIndexes[0]}/{fileRequest.File.Length}");
                                    context.Response.ContentType = fileRequest.ContentType;
                                } else {
                                    context.Response.ContentType =
                                        $"multipart/byteranges; boundary={fileRequest.Boundary}";
                                }

                                context.Response.StatusCode = (int)HttpStatusCode.PartialContent;
                                var fileStream = fileRequest.File.OpenRead();
                                fileStream.Position = fileRequest.RangesStartIndexes[0];
                                return fileStream;
                            }
                        }
                    } else {
                        Log.LogDebug("File not found: {0}", file);
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        //return $"File not found: {file}";
                    }
                } catch (Exception exception) {
                    Log.LogError(exception, exception.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            } else {
                Log.LogDebug("Incorrect parameter: {0}", file);

                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }

            return null;
        }

        public override string Handle(HttpContext context) {
            Log.LogDebug("Handle get file");
            return string.Empty;
        }

        private static bool ValidateRanges(FileRequest fileRequest, HttpResponse response) {
            long fileLength = fileRequest.File.Length;

            for (int i = 0; i < fileRequest.RangesStartIndexes.Length; i++) {
                if ((fileRequest.RangesStartIndexes[i] > fileLength - 1)
                    || (fileRequest.RangesEndIndexes[i] > fileLength - 1)
                    || (fileRequest.RangesStartIndexes[i] < 0)
                    || (fileRequest.RangesEndIndexes[i] < 0)
                    || (fileRequest.RangesEndIndexes[i] < fileRequest.RangesStartIndexes[i])) {
                    fileRequest.RangesEndIndexes[i] = 0;
                    response.StatusCode = (int) HttpStatusCode.NotAcceptable;
                    response.Headers.Add("Content-Range", $"bytes */{fileLength}");

                    return false;
                }
            }

            return true;
        }

        private static bool ValidateModificationDate(FileRequest fileRequest, HttpContext context) {
            if (context.Request.Headers.ContainsKey("If-Modified-Since")) {
                string modifiedSinceHeader = context.Request.Headers["If-Modified-Since"];
                if (!string.IsNullOrEmpty(modifiedSinceHeader)) {
                    var modifiedSinceDate = FileRequest.ParseHttpDateHeader(context, "If-Modified-Since");

                    if (fileRequest.File.LastWriteTime <= modifiedSinceDate) {
                        context.Response.StatusCode = (int) HttpStatusCode.NotAcceptable;
                        return false;
                    }
                }
            }

            if (context.Request.Headers.ContainsKey("If-Unmodified-Since")) {
                string unmodifiedSinceHeader = context.Request.Headers["If-Unmodified-Since"];

                if (string.IsNullOrEmpty(unmodifiedSinceHeader)) {
                    if (context.Request.Headers.ContainsKey("Unless-Modified-Since")) {
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

        private static bool ValidateEntityTag(FileRequest fileRequest, HttpContext context) {
            if (context.Request.Headers.ContainsKey("If-Match")) {
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

            if (context.Request.Headers.ContainsKey("If-None-Match")) {
                string noneMatchHeader = context.Request.Headers["If-None-Match"];
                if (!string.IsNullOrEmpty(noneMatchHeader)) {
                    if (noneMatchHeader == "*") {
                        context.Response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                        return false;
                    }

                    var entitiesTags = noneMatchHeader.Split(FileRequest.CommaSplitArray);
                    if (entitiesTags.All(entityTag => fileRequest.EntityTag != entityTag)) {
                        context.Response.Headers.Add("ETag", fileRequest.EntityTag);
                        context.Response.StatusCode = (int) HttpStatusCode.NotAcceptable;
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

        private readonly HttpContext _context;

        private FileRequest(HttpContext context, string file) {
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
            ContentType = MimeTypes.ContainsKey(File.Extension)
                ? MimeTypes.Get(File.Extension)
                : "application/octet-stream";
            ParseRanges();
        }

        private void ParseRanges() {
            if (_context.Request.Headers.ContainsKey("Range")) {
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

        internal static DateTime ParseHttpDateHeader(HttpContext context, string headerName) {
            if (context.Request.Headers.ContainsKey("headerName")) {
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

        public static FileRequest Create(HttpContext context, string file) {
            var fr = new FileRequest(context, file);

            fr.Parse();

            return fr;
        }
    }
}
