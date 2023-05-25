using APP1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

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
            DateTime myDateTime = DateTime.Now;

            

            // Display the name of the current culture.
            CultureInfo ci = Thread.CurrentThread.CurrentCulture;
            Debug.WriteLine("Current culture: \"{0}\"\n", ci.Name);

            // Display the long date pattern and string.
            Debug.WriteLine("Long date pattern: \"{0}\"", ci.DateTimeFormat.LongDatePattern);
            Debug.WriteLine("Long date string:  \"{0}\"\n", myDateTime.ToLongDateString());
            var us = db.user.ToList();

            return View(us);
        }
        [Route("/register")]
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
            //db.user.Where(m => m.Id == user.Id).ExecuteUpdate(m=> m.SetProperty(x=>x.roleId,x=>x.roleId-1));
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("withrole");
        }

        [Route("/home/increase/{id?}")]
        public IActionResult increase(int id)
        {
            db.user.Where(m => m.Id == id).ExecuteUpdate(m => m.SetProperty(x => x.roleId, x => x.roleId + 1));
            db.SaveChanges();
            return RedirectToAction("withrole");
        }

        [Route("/home/decrease/{id?}")]
        public IActionResult decrease(int id)
        {
            db.user.Where(m => m.Id == id).ExecuteUpdate(m => m.SetProperty(x => x.roleId, x => x.roleId - 1));
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
        public ActionResult GetUretim(cinsTurSelected cts)
        {
            cinsTurDTO? ct = null;
            if (cts.cins == null && cts.tur == null)
            {
                ct = new cinsTurDTO();
                ct.cins = db.Uretim.Select(m => m.UrunCinsi).Distinct().ToList();
                ct.tur = new List<string>();
                ct.selectedCins = "";
                ct.selectedTur = "";
                ct.maxAdet = 0;
            }
            else if (cts.cins != null && cts.tur == null)
            {
                List<Uretim> uretimList = db.Uretim.Where(m => m.UrunCinsi.Equals(cts.cins)).ToList();
                ct = new cinsTurDTO();
                ct.cins = db.Uretim.Select(m => m.UrunCinsi).Distinct().ToList();
                ct.tur = uretimList.Select(m => m.UrunTuru).Distinct().ToList();
                ct.selectedCins = cts.cins;
                ct.selectedTur = "";
                ct.maxAdet = uretimList.Sum(m => m.UrunMiktari);
            }
            else
            {
                List<Uretim> uretimList = db.Uretim.Where(m => m.UrunCinsi.Equals(cts.cins)).ToList();
                ct = new cinsTurDTO();
                ct.cins = db.Uretim.Select(m => m.UrunCinsi).Distinct().ToList();
                ct.tur = uretimList.Select(m => m.UrunTuru).Distinct().ToList();
                ct.selectedCins = cts.cins;
                ct.selectedTur = cts.tur;
                ct.maxAdet = db.Uretim.Where(m => m.UrunCinsi.Equals(cts.cins) && m.UrunTuru.Equals(cts.tur)).Sum(m => m.UrunMiktari);
            }
            return PartialView("_uretim",ct);
        }
        [Route("/uretim")]
        public IActionResult GetUretim()
        {
            cinsTurDTO? ct = new cinsTurDTO();
            ct.cins = db.Uretim.Select(m => m.UrunCinsi).Distinct().ToList();
            ct.tur = new List<string>();
            ct.selectedCins = "";
            ct.selectedTur = "";
            ct.maxAdet = 0;
            return View(ct);
        }

    }
}