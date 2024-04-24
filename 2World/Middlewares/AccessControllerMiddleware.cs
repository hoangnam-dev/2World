using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace _2World.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AccessControllerMiddleware
    {

        private readonly string[] _accessUserController = { "Auth", "Home", "Product", "Category" };
        private readonly string[] _accessGuestController = { "Auth", "Home" };
        private int ADMIN = 1;
        private int USER = 2;
        private int GUEST = 3;

        private readonly RequestDelegate _next;

        public AccessControllerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!string.IsNullOrEmpty(context.Session.GetString("UserRole")))
            {
                int roleId = int.Parse(context.Session.GetString("UserRole"));

                if (roleId != ADMIN)
                {
                    var endpoint = context.GetEndpoint();
                    var controller = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
                    string nextController = controller.ControllerName ?? "Home";
                    switch (roleId)
                    {
                        case 2:
                            if (!Array.Exists(_accessUserController, c => c == nextController))
                            {
                                context.Response.Redirect("/Home/Error");
                                return;
                            }
                            break;

                        case 3:
                            if (!Array.Exists(_accessGuestController, c => c == nextController))
                            {
                                context.Response.Redirect("/Home/Error");
                                return;
                            }
                            break;
                    }
                }
            }

            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AccessControllerMiddlewareExtensions
    {
        public static IApplicationBuilder UseAccessControllerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AccessControllerMiddleware>();
        }
    }
}
