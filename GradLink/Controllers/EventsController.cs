using GradLink.Repository.MSSQL.ORM.Context;
using GradLink.Repository.MSSQL.ORM.Entities;
using Microsoft.AspNetCore.Mvc;

namespace GradLink.Controllers
{
    public class EventsController : Controller
    {
        private readonly GradLinkDbContext db;
        public EventsController(GradLinkDbContext dbContext)
        {
            db = dbContext;
        }

        public IActionResult Index()
        {
            return View(db.Events.OrderBy(e => e.EventDate).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public IActionResult Create([Bind(Include = "Title,EventDate,Location,Description")] Event @event)
        public IActionResult Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                var exists = db.Events.Any(e => e.Title == @event.Title && e.EventDate == @event.EventDate);
                if (exists)
                {
                    ModelState.AddModelError("", "An event with the same title and date already exists.");
                    TempData["ErrorMessage"] = "An event with the same title and date already exists.";
                    return RedirectToAction("Index");
                }

                db.Events.Add(@event);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
