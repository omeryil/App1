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

            string role = "";
            if (httpContext.Session.GetString("role") != null) {
                role = httpContext.Session.GetString("role");
            }


            //Boş mu diye check ettim
            if (!String.IsNullOrEmpty(role))
            {
                //Burada hangi fonksiyona gidildiğinin path'ini okuyoruz ve console'a yazdırıyoruz..
                //Okuma işlemini fonksiyon bazlı izinleri check etmek için kullanacağız.
                System.Diagnostics.Debug.WriteLine(httpContext.Request.Path.ToString());
                
                //Burada hata dönüşünü örnekledim.
                //Sonsuz döngüye girmesin diye errors sayfası ise errors'a atma dedim yani
                //Onun dışındaki her isteği errors/403'e at dedim.
                //Login işleminden sonra her isteği errors'a atar.
                //Gör diye böyle yaptım.
                //Aşağıdakini de incele!!!
                //if(!httpContext.Request.Path.ToString().StartsWith("/errors"))
                   // httpContext.Response.Redirect("/errors/403");
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Spesifik olarak errors'a atmak istersen aşağıdakini incele
            //Login işlemi yapmadan bu url'e gidersen hataya atar.
            //Login sonrası admin değilsen ve bu url'e gidersen yine hataya atar.
            //Login sonrası admin isen url'e gider.
            //if (httpContext.Request.Path.ToString().StartsWith("/withrole") && !role.Equals("admin"))
            //    httpContext.Response.Redirect("/errors/403");
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
