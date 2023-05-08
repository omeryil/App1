using APP1.Models;
using Microsoft.AspNetCore.Mvc;

namespace APP1.Controllers
{
    public class ErrorController : Controller
    {
        [Route("errors/{statusCode}")]
        public IActionResult CustomError(int statusCode)
        {
            //Basit switch attım status code ne gelirse
            //ona göre mesajı değiştirip view'a yolladım
            var cuserr = new CustomError() { ErrorMessage = "", StatusCode = statusCode };
            switch (statusCode)
            {
                case 403:
                    cuserr.ErrorMessage = "Forbidden";
                    break;
                case 404:
                    cuserr.ErrorMessage = "Not Found";
                    break;
                default:
                    cuserr.ErrorMessage = "Unexpected Error";
                    break;
            }
            return View(cuserr);
        }
    }
}
