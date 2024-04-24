using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace _2World.Middlewares
{
    public class UserAuthMiddleware: ActionFilterAttribute
    {
        private readonly RequestDelegate _next;
        private readonly string[] _excludeUrls = { "/Auth/Login", "/Auth/Register" };
        public UserAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestPath = context.Request.Path;

            bool isExcluded = _excludeUrls.Any(url => url.Equals(requestPath, StringComparison.OrdinalIgnoreCase));

            if (!isExcluded && !context.Session.Keys.Contains("UserId"))
            {
                context.Response.Redirect("/Auth/Login");
                return;
            }

            if(context.Session.Keys.Contains("UserId") && isExcluded)
            {
                context.Response.Redirect("/Home/Index");
            }

            await _next(context);
        }

        // Access deny url /Auth/Login and /Auth/Register when user login
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var requestPath = context.HttpContext.Request.Path;
            if (!Array.Exists(_excludeUrls, url => url == requestPath))
            {
                var userId = context.HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Auth", action = "Login" }));
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
