using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RemoteFork.Network;

namespace RemoteFork.Requestes {
    public class ForkPlayerRequestHandler : BaseRequestHandler<string> {
        public const string URL_PATH = "forkplayer";

        public override async Task<string> Handle(HttpRequest request, HttpResponse response) {
            string userAgent = request.Headers["User-Agent"].ToString();
            var headers = new Dictionary<string, string> {
                {"User-Agent", userAgent}
            };

            string script =
                await HTTPUtility.GetRequestAsync("http://getlist5.obovse.ru/jsapp/app.js.php?run=js", headers);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "forkplayer");
            string filePath = Path.Combine(path, "forkplayer.js");
            
            if (string.IsNullOrEmpty(script)) {
                if (File.Exists(filePath)) {
                    script = await File.ReadAllTextAsync(filePath);
                }
            } else {
                CreateDirectoryRecursively(path);
                using (var outputFile = new StreamWriter(filePath, false)) {
                    await outputFile.WriteAsync(script);
                }
            }

            return script;
        }
        private static void CreateDirectoryRecursively(string path) {
            var pathParts = path.Split('\\');

            for (int i = 0; i < pathParts.Length; i++) {
                if (i > 0)
                    pathParts[i] = Path.Combine(pathParts[i - 1], pathParts[i]);

                if (!Directory.Exists(pathParts[i]))
                    Directory.CreateDirectory(pathParts[i]);
            }
        }
    }
}
