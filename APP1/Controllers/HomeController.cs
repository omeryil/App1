using APP1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace APP1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExampleDBContext db;
        public HomeController(ILogger<HomeController> logger,ExampleDBContext _db)
        {
            _logger = logger;
            db = _db;
        }
        //Bütün kullanıcıları getirir
        public IActionResult Index()
        {
            if (db.Database.CanConnect())
            {
                Console.WriteLine("OK");
            }
            var us = db.user.ToList();

            return View(us);
        }
        [Route("/create")]
        public IActionResult CreateUser()
        {
          
            return View();
        }
        //Rollerle kullanıcıları join ederek getirir
        [Route("/withrole/{id?}")]
        public IActionResult WithRole(int id)
        {
            if (id > 0)
            {
              
                var list = db.user.Where(m => m.Id == id).Join(db.role, m => m.roleId, n => n.Id, (user, role) => new { User = user, Role = role })
                   .Select(m => new withRoleDTO()
                   {
                       Id = m.User.Id,
                       username = m.User.username,
                       userpass = m.User.userpass,
                       name = m.User.name,
                       surname = m.User.surname,
                       role = m.Role.userrole
                   })
                   .ToList();

                return View(list);
            }
            else {
                var list = db.user.Join(db.role, m => m.roleId, n => n.Id, (user, role) => new { User = user, Role = role })
                      .Select(m => new withRoleDTO()
                      {
                          Id = m.User.Id,
                          username = m.User.username,
                          userpass = m.User.userpass,
                          name = m.User.name,
                          surname = m.User.surname,
                          role = m.Role.userrole
                      })
                      .ToList();

                return View(list);
            }
        }
        //kullanıcıyı update etmek için oluşturulan view'a id ile filtreleyip user'ı döner
        [Route("/edituser/{id?}")]
        public IActionResult EditUser(int id)
        {

            var user = db.user.Where(m => m.Id == id).FirstOrDefault(); 

            return View(user);

        }
        //Seçilen kullanıcıyı id'sine göre siler
        [Route("/home/delete/{id?}")]
        public IActionResult DeleteUser(int id)
        {
            user delete_user = db.user.Where(m => m.Id == id).FirstOrDefault();
            db.Remove(delete_user);
            db.SaveChanges();
            return RedirectToAction("withrole");

        }
        //Update edilen kullanıcının form post edildiği yer. Gelen veriye göre kullanıcıyı update eder.
        [Route("/Home/edit")]
        [HttpPost]
        public IActionResult Edit(user user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("withrole");
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}