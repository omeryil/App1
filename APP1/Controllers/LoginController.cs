using APP1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APP1.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly ExampleDBContext db;
        public LoginController(ILogger<LoginController> logger, ExampleDBContext _db)
        {
            _logger = logger;
            db = _db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [Route("/login")]
        public IActionResult LoginUserPage()
        {
            //İlk açıldığında role session'ı boş olduğu için if içerisine girmeden view'a gidecek.
            // Login işlemi sonrasında RedirectToAction'ı yine buraya yönlendirdim. role session'ı
            //artık boş olmadığı için if içerisine girecek.
            //Basit olarak session okumayı örnekledim aslında
            if(HttpContext.Session.GetString("role") != null)
            {
                var role=HttpContext.Session.GetString("role");
                System.Diagnostics.Debug.WriteLine(role);
            }
            return View();

        }
        [Route("/Login/login")]
        [HttpPost]
        public IActionResult loginUser(user user)
        {
            //Form'dan user gelecek. Role'ü alabilmek için join işlemi ile birlikte
            //user modelinden gelen username ve userpass'i where içerisinde sorguladım.
            // if (u != null) ile boş mu diye check ettim. Değilse Session içerisine yazdım.
            //Bunu da basit olarak session oluşturma olarak görebilirsin.
            var u =db.user.Where(m => m.username==user.username && m.userpass==user.userpass).Join(db.role, m => m.roleId, n => n.Id, (user, role) => new { User = user, Role = role })
                   .Select(m => new withRoleDTO()
                   {
                       Id = m.User.Id,
                       username = m.User.username,
                       userpass = m.User.userpass,
                       name = m.User.name,
                       surname = m.User.surname,
                       role = m.Role.userrole
                   })
                   .FirstOrDefault();
            if (u != null)
            {
                HttpContext.Session.SetString("role", u.role);
            }
            return RedirectToAction("LoginUserPage");
        }
    }
}
