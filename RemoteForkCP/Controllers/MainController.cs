using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemoteFork.Requestes;
using RemoteFork.Models;

namespace RemoteFork.Controllers {
    public class MainController : Controller {
        [Route(TestRequestHandler.UrlPath)]
        public ActionResult Test() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new TestRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(TreeviewRequestHandler.URL_PATH)]
        public ActionResult Treeview() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new TreeviewRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(ParseLinkRequestHandler.URL_PATH)]
        public ActionResult Parserlink() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new ParseLinkRequestHandler().Handle(HttpContext);
            return View();
        }
        
        [Route(AceStreamRequestHandler.URL_PATH)]
        public ActionResult Acestream() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new AceStreamRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(ProxyM3u8RequestHandler.UrlPath)]
        public byte[] ProxyM3U8() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            return new ProxyM3u8RequestHandler().Handle(HttpContext);
        }

        [Route(DlnaFileRequestHandler.URL_PATH)]
        public async Task<IActionResult> File(string id) {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            var stream = new DlnaFileRequestHandler().Handle(HttpContext);
            if (stream != null) {
                return new FileStreamResult(stream, HttpContext.Response.ContentType);
            } 
            return File(new byte[0], "text/html; charset=utf-8");
        }

        [Route(DlnaDirectoryRequestHandler.URL_PATH)]
        public ActionResult Directory() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new DlnaDirectoryRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(UserUrlsRequestHandler.URL_PATH)]
        public ActionResult UserUrls() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new UserUrlsRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(PluginRequestHandler.URL_PATH)]
        public ActionResult Plugin() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new PluginRequestHandler().Handle(HttpContext);
            return View();
        }

        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}