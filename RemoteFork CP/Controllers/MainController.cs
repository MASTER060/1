using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemoteFork.Requestes;
using RemoteFork_CP.Models;

namespace RemoteFork.Controllers {
    public class MainController : Controller {
        [Route(TestRequestHandler.UrlPath)]
        public ActionResult Test() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new TestRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(TreeviewRequestHandler.UrlPath)]
        public ActionResult Treeview() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new TreeviewRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(ParseLinkRequestHandler.UrlPath)]
        public ActionResult Parserlink() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new ParseLinkRequestHandler().Handle(HttpContext);
            return View();
        }
        
        [Route(AceStreamRequestHandler.UrlPath)]
        public ActionResult Acestream() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new AceStreamRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(ProxyM3u8RequestHandler.UrlPath)]
        public ActionResult ProxyM3U8() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new ProxyM3u8RequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(DlnaFileRequestHandler.UrlPath)]
        public async Task<IActionResult> File(string id) {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            var stream = new DlnaFileRequestHandler().HandleStream(HttpContext);
            if (stream != null) {
                return new FileStreamResult(stream, HttpContext.Response.ContentType);
            } 
            return File(new byte[0], "text/html; charset=utf-8");
        }

        [Route(DlnaDirectoryRequestHandler.UrlPath)]
        public ActionResult Directory() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new DlnaDirectoryRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(UserUrlsRequestHandler.UrlPath)]
        public ActionResult UserUrls() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = new UserUrlsRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(PluginRequestHandler.UrlPath)]
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