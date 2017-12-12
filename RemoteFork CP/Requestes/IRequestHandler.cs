using Microsoft.AspNetCore.Http;

namespace RemoteFork.Requestes {
    internal interface IRequestHandler {
        string Handle(HttpContext context);
    }
}
