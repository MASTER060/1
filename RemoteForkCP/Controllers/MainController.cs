using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemoteFork.Log.Analytics;
using RemoteFork.Requestes;
using RemoteFork.Models;

namespace RemoteFork.Controllers {
    [GoogleAnalyticsTrackEvent]
    public class MainController : Controller {
        [Route(TestRequestHandler.UrlPath)]
        public ActionResult Test() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new TestRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(DlnaRootRequestHandler.URL_PATH)]
        public ActionResult Treeview() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new DlnaRootRequestHandler().Handle(HttpContext);
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

        [Route("{text:regex(^(" + ProxyM3u8RequestHandler.UrlPath + ").*)}/{params?}")]
        public ActionResult ProxyM3U8() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            return new FileContentResult(new ProxyM3u8RequestHandler().Handle(HttpContext), "text/html");
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