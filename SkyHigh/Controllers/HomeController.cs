
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkyHigh.Data;
using SkyHigh.Models;
using System.Diagnostics;
using System;
using System.Linq;

namespace SkyHigh.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var username = HttpContext.Session.GetString("UserName");
            ViewBag.Name = username;
            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactMessage());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactMessage model)
        {
            if (ModelState.IsValid)
            {
                model.SubmittedOn = DateTime.Now;
                _context.ContactMessages.Add(model);
                _context.SaveChanges();
                TempData["ContactSuccess"] = "Your message has been sent successfully!";
                return RedirectToAction("Contact");
            }

            return View(model);
        }

        public IActionResult MyMessages()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["LoginRequired"] = "Please log in to view your messages.";
                return RedirectToAction("Login", "Account");
            }

            var messages = _context.ContactMessages
                .Where(m => m.Email == userEmail)
                .OrderByDescending(m => m.SubmittedOn)
                .ToList();

            return View(messages);
        }
    }
}




