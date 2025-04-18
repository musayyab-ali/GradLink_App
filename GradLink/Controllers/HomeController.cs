using System.Diagnostics;
using GradLink.Model.ViewModel;
using GradLink.Models;
using GradLink.Repository.MSSQL.ORM.Context;
using GradLink.Repository.MSSQL.ORM.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GradLink.Controllers
{
    public class HomeController : Controller
    {
        private readonly GradLinkDbContext db;
        public HomeController(GradLinkDbContext dbContext)
        {
            db = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            var model = new ContactUsViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitContactForm(ContactUsViewModel contactForm)
        {
            if (ModelState.IsValid)
            {
                var existingContact = db.ContactUs
                    .FirstOrDefault(c => c.Email == contactForm.Email);

                if (existingContact != null)
                {
                    ModelState.AddModelError("Email", "This email has already been used to send a message.");
                    TempData["ErrorMessage"] = "This email has already been used to send a message.";
                    return RedirectToAction("Contact", "Home");
                }

                var newContact = new ContactU
                {
                    Name = contactForm.Name,
                    Email = contactForm.Email,
                    Subject = contactForm.Subject,
                    Message = contactForm.Message
                };

                db.ContactUs.Add(newContact);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your message has been sent successfully!";
                return RedirectToAction("Contact", "Home");
            }

            return View(contactForm);
        }

        // POST: Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subscribe(string email)
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "You need to log in before subscribing.";
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "Please provide a valid email address.";
                return RedirectToAction("Index");
            }

            var existingSubscription = db.NewsletterSubscriptions.FirstOrDefault(n => n.Email == email);
            if (existingSubscription != null)
            {
                TempData["ErrorMessage"] = "This email is already subscribed.";
                return RedirectToAction("Index");
            }

            var subscription = new NewsletterSubscription
            {
                Email = email,
                DateSubscribed = DateTime.Now
            };

            db.NewsletterSubscriptions.Add(subscription);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Thank you for subscribing!";
            return RedirectToAction("Index");
        }
    }
}
