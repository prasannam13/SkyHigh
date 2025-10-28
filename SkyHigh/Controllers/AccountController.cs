
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using SkyHigh.Controllers;
using SkyHigh.Data;
using SkyHigh.Models;
using System.Net.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkyHigh.Data;
using SkyHigh.Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;   
using System.Text;

namespace SkyHigh.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string defaultProfilePic = "/ProfilePic/avtar.jpg"; 

        public AccountController(ApplicationDbContext context) => _context = context;

        public IActionResult Register()
        {
            ViewBag.RegistrationSuccess = false;
            return View(new User());
        }

        [HttpPost]
        public IActionResult Register(User user, string password)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
            }

            ModelState.Remove("PasswordHash");

            if (ModelState.IsValid)
            {
                user.PasswordHash = SkyHigh.Models.User.HashPassword(password);
                user.Role = "User";
                user.ProfilePicPath = defaultProfilePic; // Assign default pic for new users
                _context.Users.Add(user);
                _context.SaveChanges();
                ViewBag.RegistrationSuccess = true;
                return View(new User());
            }

            ViewBag.RegistrationSuccess = false;
            return View(user);
        }
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return RedirectToAction("Login");

            var picPath = string.IsNullOrEmpty(user.ProfilePicPath) ? "/ProfilePic/avtar.jpg" : user.ProfilePicPath;
            HttpContext.Session.SetString("ProfilePicPath", picPath);
            ViewBag.ProfilePicPath = picPath;
            return View(user);
        }

        [HttpPost]

        public IActionResult UpdateProfile(
    [FromForm] string Name,
    [FromForm] string Gender,
    [FromForm] string Phone,
    [FromForm] DateTime DOB,
    [FromForm] IFormFile ProfilePic,
    [FromForm] string removeProfilePic
)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "Session expired." });

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return Json(new { success = false, message = "User not found." });

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Phone) || DOB == default)
            {
                return Json(new { success = false, message = "All fields are required." });
            }

            user.Name = Name;
            user.Gender = Gender;
            user.Phone = Phone;
            user.DOB = DOB;

            string defaultProfilePic = "/ProfilePic/avtar.jpg";

            // Remove profile picture if requested
            if (removeProfilePic == "true")
            {
                // Remove old file if not default
                if (!string.IsNullOrEmpty(user.ProfilePicPath) && user.ProfilePicPath != defaultProfilePic)
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePicPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }
                user.ProfilePicPath = defaultProfilePic;
            }
            // Handle upload
            else if (ProfilePic != null && ProfilePic.Length > 0)
            {
                var ext = Path.GetExtension(ProfilePic.FileName);
                var fileName = $"{user.UserId}_{DateTime.Now.Ticks}{ext}";
                var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfilePic");
                Directory.CreateDirectory(directory);
                var path = Path.Combine(directory, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    ProfilePic.CopyTo(stream);
                }

                // Remove previous custom pic if present
                if (!string.IsNullOrEmpty(user.ProfilePicPath) && user.ProfilePicPath != defaultProfilePic)
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePicPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldImagePath))
                        System.IO.File.Delete(oldImagePath);
                }

                user.ProfilePicPath = "/ProfilePic/" + fileName;
            }

            _context.SaveChanges();

            // Always update session after change
            var picPath = string.IsNullOrEmpty(user.ProfilePicPath) ? defaultProfilePic : user.ProfilePicPath;
            HttpContext.Session.SetString("ProfilePicPath", picPath);

            return Json(new
            {
                success = true,
                name = user.Name,
                gender = user.Gender,
                dob = user.DOB.ToString("yyyy-MM-dd"),
                phone = user.Phone,
                profilePicPath = picPath
            });
        }

        public IActionResult Login() => View();

        [HttpPost]

        [ValidateAntiForgeryToken]

        public IActionResult Login(LoginViewModel model)

        {
 

            if (!ModelState.IsValid)

            {

                // if validations fail, show same page with errors

                return View(model);

            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user == null)

            {

                ViewBag.Message = "❌ Email doesn’t exist.";

                return View(model);

            }

            var hash = SkyHigh.Models.User.HashPassword(model.Password);

            if (user.PasswordHash != hash)

            {

                ViewBag.Message = "❌ Invalid password. Please try again.";

                return View(model);

            }

            // ✅ Credentials valid — set session

            HttpContext.Session.SetInt32("UserId", user.UserId);

            HttpContext.Session.SetString("Role", user.Role);

            HttpContext.Session.SetString("UserName", user.Name);

            HttpContext.Session.SetString("ProfilePicPath",

                !string.IsNullOrEmpty(user.ProfilePicPath) ? user.ProfilePicPath : "/ProfilePic/avtar.jpg");

            HttpContext.Session.SetString("UserEmail", user.Email);

            //_logger.LogAction("Login Success", user.Email);

            return RedirectToAction("Index", "Home");

        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public IActionResult ResetPassword()
        {
            return View();
        }


       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            // Ensure the form data is valid
            if (!ModelState.IsValid)
                return View(model);

            // Find the user by email (fake emails still allowed)
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                ViewBag.ResetError = "❌ No account found with that email.";
                return View(model);
            }

            // Check if new password and confirm password match
            if (model.NewPassword != model.ConfirmPassword)
            {
                ViewBag.ResetError = "❌ Passwords do not match.";
                return View(model);
            }

            // Update password
            user.PasswordHash = SkyHigh.Models.User.HashPassword(model.NewPassword);
            _context.SaveChanges();

            ViewBag.ResetSuccess = "✅ Your password has been successfully reset!";
            return View(new ResetPasswordViewModel());
        }

      
    }
}


