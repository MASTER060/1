using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RemoteFork.Log.Analytics;
using RemoteFork.Requestes;
using RemoteFork.Models;

namespace RemoteFork.Controllers {
    [GoogleAnalyticsTrackEvent]
    public class MainController : Controller {
        [Route(TestRequestHandler.URL_PATH)]
        public async Task<ActionResult> Test() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = await new TestRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(DlnaRootRequestHandler.URL_PATH)]
        public async Task<ActionResult> Treeview() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = await new DlnaRootRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(ParseLinkRequestHandler.URL_PATH)]
        public async Task<ActionResult> Parserlink() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = await new ParseLinkRequestHandler().Handle(HttpContext);
            return View();
        }
        
        [Route(AceStreamRequestHandler.URL_PATH)]
        public async Task<ActionResult> Acestream() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = await new AceStreamRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route("{text:regex(^(" + ProxyM3u8RequestHandler.UrlPath + ").*)}/{params?}")]
        public async Task<ActionResult> ProxyM3U8() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            return new FileContentResult(await new ProxyM3u8RequestHandler().Handle(HttpContext), "text/html");
        }

        [Route(DlnaFileRequestHandler.URL_PATH)]
        public async Task<IActionResult> File(string id) {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            var stream = await new DlnaFileRequestHandler().Handle(HttpContext);
            if (stream != null) {
                return new FileStreamResult(stream, HttpContext.Response.ContentType);
            } 
            return File(new byte[0], "text/html; charset=utf-8");
        }

        [Route(DlnaDirectoryRequestHandler.URL_PATH)]
        public async Task<ActionResult> Directory() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = await new DlnaDirectoryRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(UserUrlsRequestHandler.URL_PATH)]
        public async Task<ActionResult> UserUrls() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = await new UserUrlsRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(PluginRequestHandler.URL_PATH)]
        public async Task<ActionResult> Plugin() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = await new PluginRequestHandler().Handle(HttpContext);
            return View();
        }

        [Route(ForkPlayerRequestHandler.URL_PATH)]
        public async Task<ActionResult> ForkPlayer() {
            HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            ViewData["Message"] = await new ForkPlayerRequestHandler().Handle(HttpContext);
            return View();
        }

        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}