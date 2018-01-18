using System;
using System.Linq;
using System.Text.RegularExpressions;
using RemoteFork.Network;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace RemoteFork.Requestes {

    public class ParseLinkRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "parserlink";

        public override string Handle(HttpRequest request, HttpResponse response) {
            string result = string.Empty;
            Log.LogDebug(request.Host.ToUriComponent());
            var requestStrings = HttpUtility.UrlDecode(request.QueryString.Value)?.Substring(1).Split('|');
            if (request.Method == "POST") {
                var getPostParam = new StreamReader(request.Body, true);
                string postData = getPostParam.ReadToEnd();
                requestStrings = HttpUtility.UrlDecode(postData).Substring(2).Split('|');
            }
            if (requestStrings != null) {
                Log.LogDebug("Parsing: {0}", requestStrings[0]);

                string curlResponse = requestStrings[0].StartsWith("curl")
                    ? CurlRequest(requestStrings[0])
                    : HTTPUtility.GetRequest(requestStrings[0]);

                if (requestStrings.Length == 1) {
                    result = curlResponse;
                } else {
                    if (!requestStrings[1].Contains(".*?")) {
                        if (string.IsNullOrEmpty(requestStrings[1]) && string.IsNullOrEmpty(requestStrings[2])) {
                            result = curlResponse;
                        } else {
                            int num1 = curlResponse.IndexOf(requestStrings[1], StringComparison.Ordinal);
                            if (num1 == -1) {
                                result = string.Empty;
                            } else {
                                num1 += requestStrings[1].Length;
                                int num2 = curlResponse.IndexOf(requestStrings[2], num1, StringComparison.Ordinal);
                                result = num2 == -1 ? string.Empty : curlResponse.Substring(num1, num2 - num1);
                            }
                        }
                    } else {
                        Log.LogDebug("ParseLinkRequest: {0}", requestStrings[1] + "(.*?)" + requestStrings[2]);

                        string pattern = requestStrings[1] + "(.*?)" + requestStrings[2];
                        var regex = new Regex(pattern, RegexOptions.Multiline);
                        var match = regex.Match(curlResponse);
                        if (match.Success) result = match.Groups[1].Captures[0].ToString();
                    }
                }
            }

            return result;
        }

        public string CurlRequest(string text) {
            string result;
            if (text.StartsWith("curlorig")) {
                Log.LogDebug("curlorig " + text.Substring(9));

                result = HTTPUtility.GetRequest(text.Substring(9));
            } else {
                bool verbose = text.IndexOf(" -i", StringComparison.Ordinal) > 0;
                bool autoredirect = text.IndexOf(" -L", StringComparison.Ordinal) > 0;

                string url = Regex.Match(text, "(?:\")(.*?)(?=\")").Groups[1].Value;
                var matches = Regex.Matches(text, "(?:-H\\s\")(.*?)(?=\")");
                var header = (
                    from Match match in matches
                    select match.Groups
                    into groups
                    where groups.Count > 1
                    select groups[1].Value
                    into value
                    where value.Contains(": ")
                    select value).ToDictionary(value => value.Remove(value.IndexOf(": ", StringComparison.Ordinal)),
                    value => value.Substring(value.IndexOf(": ", StringComparison.Ordinal) + 2));
                if (text.Contains("--data")) {
                    Log.LogDebug("POST DATA");
                    try {
                        string dataString = Regex.Match(text, "(?:--data\\s\")(.*?)(?=\")").Groups[1].Value;
                        result = HTTPUtility.PostRequest(url, dataString, header, verbose, autoredirect);
                    } catch (Exception e) {
                        result = e.ToString();
                        Log.LogDebug(e.ToString());
                    }
                } else {
                    Log.LogInformation(url);

                    result = HTTPUtility.GetRequest(url, header, verbose, autoredirect);
                }
            }

            return result;
        }
    }
}
