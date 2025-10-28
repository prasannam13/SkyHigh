
using Microsoft.AspNetCore.Mvc;
using SkyHigh.Data;
using System;
using System.Linq;

namespace SkyHigh.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ViewMessages()
        {
            var messages = _context.ContactMessages.OrderByDescending(m => m.SubmittedOn).ToList();
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReplyMessage(int id, string AdminReply)
        {
            var msg = _context.ContactMessages.FirstOrDefault(x => x.Id == id);
            if (msg == null)
                return NotFound();

            msg.AdminReply = AdminReply;
            msg.RepliedOn = DateTime.Now;
            _context.SaveChanges();

            TempData["ReplySuccess"] = "Reply sent successfully!";
            return RedirectToAction("ViewMessages");
        }
    }
}

