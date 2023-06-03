using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Runtime.Remoting;
using Microsoft.Data.SqlClient;
using web_ver_2.Models;
using web_ver_2.Data;

namespace web_ver_2.Controllers
{
	
	public class HomeController : Controller
    {
	    private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
			if (HttpContext.Session.GetString("UserName") == null)
            {
                return View();
            }
			else
			{
				return RedirectToAction("Home", "User");
			}
		}
        //GET
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User obj)
        {
            if (ModelState.IsValid)
            {
                obj.Password = HashString(obj.Password);
                var objFromDb = _db.User.Find(obj.Email);
                if (objFromDb == null)
                {
                    ModelState.AddModelError("Email", "Email isn't registered");
	                return View();
                }
				if (objFromDb.Password != obj.Password)
                {
					ModelState.AddModelError("Password", "Incorrect Password"); 
					return View();
				}
				
                HttpContext.Session.SetString("UserName", obj.Email);
                return RedirectToAction("Home","User");
            }

            return View();
        }


        //GET
        public IActionResult Register()
        {
	        return View();
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Register(User obj)
        {
            if (ModelState.IsValid)
            {
	            if (obj.Email == _db.User.FirstOrDefault(u => u.Email == obj.Email)?.Email)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
                obj.Password = HashString(obj.Password);
                try
                {
	                _db.Add(obj);
	                _db.SaveChanges();
                }
                catch (Exception)
                {
					return View();
                }
            }
			ModelState.Clear();
			ViewBag.Error = "";
			ViewBag.Success = obj.Email + " successfully registered";
			return RedirectToAction("Login");
		}
        public IActionResult Index()
        {
            return View();
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



		static string HashString(string text, string salt = "")
		{
			if (String.IsNullOrEmpty(text))
			{
				return String.Empty;
			}

			// Uses SHA256 to create the hash
			using (var sha = new System.Security.Cryptography.SHA256Managed())
			{
				// Convert the string to a byte array first, to be processed
				byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
				byte[] hashBytes = sha.ComputeHash(textBytes);

				// Convert back to a string, removing the '-' that BitConverter adds
				string hash = BitConverter
					.ToString(hashBytes)
					.Replace("-", String.Empty);

				return hash;
			}
		}
	}
}