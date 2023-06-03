using System.Drawing;
using System.IO.Enumeration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web;
using Microsoft.Extensions.Configuration;
using System.Text.Encodings.Web;
using Amazon;
using web_ver_2.Models;
using web_ver_2.Utilities;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using File = web_ver_2.Models.File;
using web_ver_2.Data;

namespace web_ver_2.Controllers
{
    public class UserController : Controller
    {
	    private readonly ApplicationDbContext _db;

	    private readonly IConfiguration _conf;

	    public UserController(ApplicationDbContext db, IConfiguration conf)
	    {
		    _db = db;
			_conf = conf;
	    }

		// GET: UserController
		[Authentication]
        public IActionResult UploadFileView()
        {
            return View();
        }
        public async Task<IActionResult> UploadFile(IFormFile file, bool Status)
        {
			
	        using (var client = new AmazonS3Client(_conf.GetSection("AWS")["AccessKey"], _conf.GetSection("AWS")["SecretKey"],
		               RegionEndpoint.APSoutheast1))
	        {
		        using (var newMemoryStream = new MemoryStream())
		        {
			        file.CopyTo(newMemoryStream);
			        var equest = new TransferUtilityUploadRequest()
			        {
				        BucketName = "ienbucket",
				        Key = file.FileName,
				        InputStream = newMemoryStream,
				        ContentType = file.ContentType,
				        CannedACL = S3CannedACL.PublicRead
			        };

			        var fileTransferUtility = new TransferUtility(client);
			        await fileTransferUtility.UploadAsync(equest);
		        }
				//Adding uploaded file to database
				File dbFile = new File();
				dbFile.URL = HashString(file.FileName + HttpContext.Session.GetString("UserMame"));
				dbFile.Name = file.FileName;
				dbFile.Type = file.ContentType;
				if (Status == true)
				{
					dbFile.Status = "DeleteOnView";
				}
				else
				{
					dbFile.Status = "Active";
				}
				dbFile.Email = HttpContext.Session.GetString("UserName");
				_db.File.Add(dbFile);
				_db.SaveChanges();
				return RedirectToAction("Home", "User");
	        }
        }

        [Authentication]
        public IActionResult Home()
        {
			return View();
        }

        public ActionResult Logout()
        {

	        HttpContext.Session.Clear();
	        HttpContext.Session.Remove("UserName");

            return RedirectToAction("Index", "Home");   
		}
		// POST: UserController/UploadFile



		// POST: UserController/Create
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Create(IFormCollection collection)
		//{
		//    try
		//    {
		//        return RedirectToAction(nameof(Index));
		//    }
		//    catch
		//    {
		//        return View();
		//    }
		//}
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
