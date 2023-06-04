using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Web;
using Amazon;
using Microsoft.Data.SqlClient;
using web_ver_2.Models;
using web_ver_2.Data;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using Amazon.S3;
using File = web_ver_2.Models.File;

namespace web_ver_2.Controllers
{
	
	public class HomeController : Controller
    {
		private readonly IConfiguration _conf;


		private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db, IConfiguration conf)
        {
            _db = db;
            _conf = conf;
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
				//Check if password is correct
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
				//Check email if already exists
	            if (obj.Email == _db.User.FirstOrDefault(u => u.Email == obj.Email)?.Email)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                }
				//Hash password
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
			if (HttpContext.Session.GetString("UserName") == null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Home", "User");
			}
		}

        public IActionResult File(string id)
        {
			//get file data from database
	        var fileData = new Models.File();
            fileData = _db.File.FirstOrDefault(u => u.URL == id);
            if (fileData == null || fileData.Status == "Deleted") //if file is deleted or doesn't exist
            {
				return RedirectToAction("Index", "Home");
			}
            return View(fileData);
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

        public async Task<IActionResult> DownloadFile(string id)
        {
			using (var client = new AmazonS3Client(_conf.GetSection("AWS")["AccessKey"], _conf.GetSection("AWS")["SecretKey"],
				       RegionEndpoint.APSoutheast1))
			{
				//delete file from aws server
				var transferUtility = new TransferUtility(client);
				try
				{
					var response = await transferUtility.S3Client.GetObjectAsync(_conf.GetSection("AWS")["BucketName"], id);
					if (response.ResponseStream == null)
					{
						return RedirectToAction("Home", "User");
					}
					var file = new File();
					file = _db.File.FirstOrDefault(u => u.Name == id);
					if (file.Status == "DeleteOnView")
					{
						//delete file from aws server
						var deleteObjectRequest = new DeleteObjectRequest
						{
							BucketName = _conf.GetSection("AWS")["BucketName"],
							Key = id
						};
						await client.DeleteObjectAsync(deleteObjectRequest);

						//edit status to deleted
						_db.File.FirstOrDefault(u => u.Name == id).Status = "Deleted";

						_db.SaveChanges();
					}
					//return string if file format is txt
					if (response.Headers["Content-Type"] == "text/plain")
					{
						using (var reader = new StreamReader(response.ResponseStream))
						{
							var content = await reader.ReadToEndAsync();
							return Content(content, "text/plain", Encoding.UTF8);
						}
					}

					return File(response.ResponseStream, response.Headers["Content-Type"], id);
				}
				catch (Exception)
				{
					return RedirectToAction("Index", "Home");
				}
				//look for file status in database

			}
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