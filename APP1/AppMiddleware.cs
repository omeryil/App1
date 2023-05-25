using APP1.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace APP1
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AppMiddleware
    {
        private readonly RequestDelegate _next;

        public AppMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext,
             ExampleDBContext db)
        {
            //Session'dan role verisini okudum.
            //string role = httpContext.Session.GetString("role") != null?
            //   httpContext.Session.GetString("role"):"";

            string role = "0";
            if (httpContext.Session.GetString("role") != null) {
                role = httpContext.Session.GetString("role");
            }

            var path = httpContext.Request.Path.ToString();
            var urlList=db.permission.Where(m=>m.roleId==int.Parse(role)).Select(m=>m.urlId).ToList();
            var urlVar = db.urls.Where(m => urlList.Contains(m.Id) && m.url.Equals(path)).Any();
            if (!urlVar)
            {
                if (!httpContext.Request.Path.ToString().StartsWith("/errors"))
                    httpContext.Response.Redirect("/errors/403");
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AppMiddlewareExtensions
    {
        public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AppMiddleware>();
        }
    }
}
