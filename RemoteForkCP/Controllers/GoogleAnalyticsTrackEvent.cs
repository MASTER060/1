using Microsoft.AspNetCore.Mvc.Filters;

namespace RemoteFork.Log.Analytics {
    public class GoogleAnalyticsTrackEvent : ActionFilterAttribute {
        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            GAApi.TrackPageview(filterContext.HttpContext.Request.Path + filterContext.HttpContext.Request.QueryString);
        }
    }
}
