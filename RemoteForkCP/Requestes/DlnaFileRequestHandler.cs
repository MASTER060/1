using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Http;
using RemoteFork.Tools;

namespace RemoteFork.Requestes {
    public class DlnaFileRequestHandler : BaseRequestHandler<Stream> {
        public const string URL_PATH = "file";

        public override Stream Handle(HttpRequest request, HttpResponse response) {
            Log.LogDebug("HandleStream get file");

            string file = string.Empty;

            if (request.Query.ContainsKey(string.Empty)) {
                file = request.Query[string.Empty][0];
                file = HttpUtility.UrlDecode(file);
            }

            if (!string.IsNullOrEmpty(file)) {
                try {
                    var fileRequest = FileRequest.Create(request, file);

                    if (fileRequest.File.Exists && Tools.Tools.CheckAccessPath(fileRequest.File.FullName)) {

                        response.Headers.Add("Accept-Ranges", "bytes");
                        if (ValidateRanges(response, fileRequest) &&
                            ValidateModificationDate(request, response, fileRequest) &&
                            ValidateEntityTag(request, response, fileRequest)) {
                            response.Headers.Add("Last-Modified", fileRequest.File.LastWriteTime.ToString("r"));
                            response.Headers["Etag"] = fileRequest.EntityTag;

                            if (!fileRequest.RangeRequest) {
                                response.ContentLength = fileRequest.File.Length;
                                response.ContentType = fileRequest.ContentType;
                                response.StatusCode = (int) HttpStatusCode.OK;

                                var fileStream = fileRequest.File.OpenRead();
                                fileStream.Position = 0;
                                return fileStream;
                            } else {
                                response.ContentLength = fileRequest.GetContentLength();

                                if (!fileRequest.MultipartRequest) {
                                    response.Headers.Add("Content-Range",
                                        $"bytes {fileRequest.RangesStartIndexes[0]}-{fileRequest.RangesEndIndexes[0]}/{fileRequest.File.Length}");
                                    response.ContentType = fileRequest.ContentType;
                                } else {
                                    response.ContentType =
                                        $"multipart/byteranges; boundary={fileRequest.Boundary}";
                                }

                                response.StatusCode = (int) HttpStatusCode.PartialContent;
                                var fileStream = fileRequest.File.OpenRead();
                                fileStream.Position = fileRequest.RangesStartIndexes[0];
                                return fileStream;
                            }
                        }
                    } else {
                        Log.LogDebug("File not found: {0}", file);
                        response.StatusCode = (int) HttpStatusCode.NotFound;
                    }
                } catch (Exception exception) {
                    Log.LogError(exception, exception.Message);
                    response.StatusCode = (int) HttpStatusCode.NotFound;
                }
            } else {
                Log.LogDebug("Incorrect parameter: {0}", file);

                response.StatusCode = (int) HttpStatusCode.NoContent;
            }

            return null;
        }

        private static bool ValidateRanges(HttpResponse response, FileRequest fileRequest) {
            long fileLength = fileRequest.File.Length;

            for (int i = 0; i < fileRequest.RangesStartIndexes.Length; i++) {
                if ((fileRequest.RangesStartIndexes[i] > fileLength - 1)
                    || (fileRequest.RangesEndIndexes[i] > fileLength - 1)
                    || (fileRequest.RangesStartIndexes[i] < 0)
                    || (fileRequest.RangesEndIndexes[i] < 0)
                    || (fileRequest.RangesEndIndexes[i] < fileRequest.RangesStartIndexes[i])) {
                    fileRequest.RangesEndIndexes[i] = 0;
                    response.StatusCode = (int) HttpStatusCode.NotModified;
                    response.Headers.Add("Content-Range", $"bytes */{fileLength}");

                    return false;
                }
            }

            return true;
        }

        private static bool ValidateModificationDate(HttpRequest request, HttpResponse response,
            FileRequest fileRequest) {
            if (request.Headers.ContainsKey("If-Modified-Since")) {
                string modifiedSinceHeader = request.Headers["If-Modified-Since"];
                if (!string.IsNullOrEmpty(modifiedSinceHeader)) {
                    var modifiedSinceDate = FileRequest.ParseHttpDateHeader(request, "If-Modified-Since");

                    if (fileRequest.File.LastWriteTime <= modifiedSinceDate) {
                        response.StatusCode = (int) HttpStatusCode.NotModified;
                        return false;
                    }
                }
            }

            if (request.Headers.ContainsKey("If-Unmodified-Since")) {
                string unmodifiedSinceHeader = request.Headers["If-Unmodified-Since"];

                if (string.IsNullOrEmpty(unmodifiedSinceHeader)) {
                    if (request.Headers.ContainsKey("Unless-Modified-Since")) {
                        unmodifiedSinceHeader = request.Headers["Unless-Modified-Since"];
                    }
                }

                if (!string.IsNullOrEmpty(unmodifiedSinceHeader)) {
                    var unmodifiedSinceDate = FileRequest.ParseHttpDateHeader(request, "If-Modified-Since");

                    if (fileRequest.File.LastWriteTime > unmodifiedSinceDate) {
                        response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool ValidateEntityTag(HttpRequest request, HttpResponse response, FileRequest fileRequest) {
            if (request.Headers.ContainsKey("If-Match")) {
                string matchHeader = request.Headers["If-Match"];
                if (matchHeader != "*") {
                    var entitiesTags = matchHeader.Split(FileRequest.CommaSplitArray);
                    int entitieTagIndex = 0;

                    for (; entitieTagIndex < entitiesTags.Length; entitieTagIndex++) {
                        if (fileRequest.EntityTag == entitiesTags[entitieTagIndex]) {
                            break;
                        }
                    }

                    if (entitieTagIndex >= entitiesTags.Length) {
                        response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                        return false;
                    }
                }
            }

            if (request.Headers.ContainsKey("If-None-Match")) {
                string noneMatchHeader = request.Headers["If-None-Match"];
                if (!string.IsNullOrEmpty(noneMatchHeader)) {
                    if (noneMatchHeader == "*") {
                        response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                        return false;
                    }

                    var entitiesTags = noneMatchHeader.Split(FileRequest.CommaSplitArray);
                    if (entitiesTags.All(entityTag => fileRequest.EntityTag == entityTag)) {
                        response.Headers["Etag"] = fileRequest.EntityTag;
                        response.StatusCode = (int) HttpStatusCode.NotModified;
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public sealed class FileRequest {
        private const string HEADER_IF_RANGE = "If-Range";

        private static readonly MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();

        private static readonly string[] HttpDateFormats =
            {"r", "dddd, dd-MMM-yy HH':'mm':'ss 'GMT'", "ddd MMM d HH':'mm':'ss yyyy"};

        private readonly HttpRequest _request;

        private static readonly char[] DashSplitArray = {'-'};

        public static readonly char[] CommaSplitArray = {','};

        public string Boundary { get; } = "---------------------------" + DateTime.Now.Ticks.ToString("x");

        public FileInfo File { get; }

        public bool RangeRequest { get; private set; }
        public bool MultipartRequest { get; private set; }

        public string EntityTag { get; private set; }
        public string ContentType { get; private set; }

        public long[] RangesStartIndexes { get; private set; }
        public long[] RangesEndIndexes { get; private set; }

        private FileRequest(HttpRequest request, string file) {
            _request = request;
            File = new FileInfo(new Uri(file).LocalPath);
        }

        private void Parse() {
            EntityTag = GenerateEntityTag();
            ContentType = MimeTypes.ContainsKey(File.Extension)
                ? MimeTypes.Get(File.Extension)
                : "application/octet-stream";
            ParseRanges();
        }

        private void ParseRanges() {
            if (_request.Headers.ContainsKey("Range")) {
                string rangesHeader = _request.Headers["Range"];
                string ifRangeHeader = _request.Headers[HEADER_IF_RANGE];
                var ifRangeHeaderDate = ParseHttpDateHeader(_request, HEADER_IF_RANGE);

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

        internal static DateTime ParseHttpDateHeader(HttpRequest request, string headerName) {
            if (request.Headers.ContainsKey("headerName")) {
                string ifRangeHeader = request.Headers[headerName];
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

        public static FileRequest Create(HttpRequest request, string file) {
            var fr = new FileRequest(request, file);

            fr.Parse();

            return fr;
        }
    }
}
