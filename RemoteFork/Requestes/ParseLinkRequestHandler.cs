using System;
using System.Linq;
using System.Text.RegularExpressions;
using RemoteFork.Network;
using Unosquare.Net;
using System.Diagnostics;
using System.IO;
using NLog;

namespace RemoteFork.Requestes {

    internal class ParseLinkRequestHandler : BaseRequestHandler {
        private static readonly ILogger Log = LogManager.GetLogger("ParseLinkRequestHandler", typeof(ParseLinkRequestHandler));

        internal static readonly string UrlPath = "/parserlink";

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response) {
            var result = string.Empty;
            Console.WriteLine(request.RawUrl);
            var requestStrings = System.Web.HttpUtility.UrlDecode(request.RawUrl)?.Substring(UrlPath.Length + 1).Split('|');
            if (request.HttpMethod == "POST")
            {
                StreamReader getPostParam = new StreamReader(request.InputStream, true);
                var postData = getPostParam.ReadToEnd();
                //Console.WriteLine("POST "+ postData);
                requestStrings = System.Web.HttpUtility.UrlDecode(postData)?.Substring(2).Split('|');
            }
            if (requestStrings != null) {
                Log.Debug("Parsing: {0}", requestStrings[0]);

                var curlResponse = requestStrings[0].StartsWith("curl")
                    ? CurlRequest(requestStrings[0])
                    : HttpUtility.GetRequest(requestStrings[0]);

                if (requestStrings.Length == 1) {
                    result = curlResponse;
                } else {
                    if (!requestStrings[1].Contains(".*?")) {
                        if (string.IsNullOrEmpty(requestStrings[1]) && string.IsNullOrEmpty(requestStrings[2])) {
                            result = curlResponse;
                        } else {
                            var num1 = curlResponse.IndexOf(requestStrings[1], StringComparison.Ordinal);
                            if (num1 == -1) {
                                result = string.Empty;
                            } else {
                                num1 += requestStrings[1].Length;
                                var num2 = curlResponse.IndexOf(requestStrings[2], num1, StringComparison.Ordinal);
                                result = num2 == -1 ? string.Empty : curlResponse.Substring(num1, num2 - num1);
                            }
                        }
                    } else {
                        Log.Debug("ParseLinkRequest: {0}", requestStrings[1] + "(.*?)" + requestStrings[2]);

                        var pattern = requestStrings[1] + "(.*?)" + requestStrings[2];
                        var regex = new Regex(pattern, RegexOptions.Multiline);
                        var match = regex.Match(curlResponse);
                        if (match.Success) result = match.Groups[1].Captures[0].ToString();
                    }
                }
            }

            WriteResponse(response, result);
        }
        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
        }
        public string CurlRequest(string text) {
            string result;
            if (text.IndexOf("curlorig") == 0)
            {
                Console.WriteLine("curlorig "+ text.Substring(9));
                Process process = new Process();
                process.StartInfo.FileName = "curlorig.exe";
                process.StartInfo.Arguments = "/c "+text.Substring(9);
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                //* Set ONLY ONE handler here.
                process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
                //* Start process
                process.Start();
                //* Read one element asynchronously
                process.BeginErrorReadLine();
                //* Read the other one synchronously
                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit(45000);
                return result;
            }
            var verbose = text.IndexOf(" -i", StringComparison.Ordinal) > 0;
            var autoredirect = text.IndexOf(" -L", StringComparison.Ordinal) > 0;

            var url = Regex.Match(text, "(?:\")(.*?)(?=\")").Groups[1].Value;
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
                Console.WriteLine("POST DATA");
                try { 
                  var dataString = Regex.Match(text, "(?:--data\\s\")(.*?)(?=\")").Groups[1].Value;
                  result = HttpUtility.PostRequest(url, dataString, header, verbose,autoredirect);
                }
                catch(Exception e)
                {
                    result = e.ToString();
                    Console.WriteLine(e.ToString());
                }
            } else {
                Log.Info(url);

                result = HttpUtility.GetRequest(url, header, verbose,false, autoredirect);
            }

            return result;
        }
    }

   
}