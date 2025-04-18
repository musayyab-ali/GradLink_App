using GradLink.Model.ViewModel.QuestionAnswer;
using GradLink.Repository.MSSQL.ORM.Context;
using GradLink.Repository.MSSQL.ORM.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradLink.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly GradLinkDbContext db;
        public DashBoardController(GradLinkDbContext dbContext)
        {
            db = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Members()
        {
            return View();
        }

        public IActionResult Announcements()
        {
            return View();
        }
        public IActionResult QNA()
        {
            var questions = db.Questions.Include("Answers").OrderByDescending(q => q.CreatedOn).ToList()
                .Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    UserName = q.UserName,
                    QuestionText = q.QuestionText,
                    CreatedOn = q.CreatedOn,
                    Answers = q.Answers
                        .OrderBy(a => a.CreatedOn)
                        .Select(a => new AnswerViewModel
                        {
                            Id = a.Id,
                            AnsweredBy = a.AnsweredBy,
                            AnswerText = a.AnswerText,
                            CreatedOn = a.CreatedOn
                        }).ToList()
                }).ToList();

            return View(questions);
        }


        public IActionResult Ask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ask(string QuestionText)
        {
            if (string.IsNullOrWhiteSpace(QuestionText))
            {
                ModelState.AddModelError("", "Question cannot be empty.");
                TempData["ErrorMessage"] = "Question cannot be empty";
                return RedirectToAction("QNA");
            }

            var userName = User.Identity.Name;

            var question = new Question
            {
                UserName = userName,
                QuestionText = QuestionText,
                CreatedOn = DateTime.Now
            };

            db.Questions.Add(question);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Your question has been submitted.";
            return RedirectToAction("QNA");
        }

        public IActionResult Answer(int id)
        {
            var question = db.Questions.Find(id);
            if (question == null)
            {
                //return HttpNotFound();
            }
            ViewBag.QuestionText = question.QuestionText;
            ViewBag.QuestionId = question.Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Answer(int QuestionId, string AnswerText)
        {
            if (string.IsNullOrEmpty(AnswerText))
            {
                TempData["ErrorMessage"] = "Answer cannot be empty.";
                return RedirectToAction("QNA", "DashBoard");
            }

            var answer = new Answer
            {
                QuestionId = QuestionId,
                AnswerText = AnswerText,
                AnsweredBy = User.Identity.Name,
                CreatedOn = DateTime.Now
            };

            db.Answers.Add(answer);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Answer posted successfully!";
            return RedirectToAction("QNA");
        }


        public IActionResult Resources()
        {
            return View();
        }
        public IActionResult Career()
        {
            var adviceList = db.CareerAdvices.OrderByDescending(c => c.CreatedDate).ToList();
            return View(adviceList);
        }

        //public ActionResult CareerAdvice([Bind(Include = "Title,Description")] CareerAdvice advice)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var exists = db.CareerAdvices.Any(e => e.Title == advice.Title && e.CreatedDate == @event.EventDate);
        //        if (exists)
        //        {
        //            ModelState.AddModelError("", "An event with the same title and date already exists.");
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    return View(advice);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CareerAdvice(CareerAdvice advice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(advice.Title) || string.IsNullOrWhiteSpace(advice.Description))
                    {
                        ModelState.AddModelError("", "Both title and description are required.");
                        TempData["ErrorMessage"] = "Both title and description are required.";
                        return RedirectToAction("Career", "DashBoard");
                    }

                    advice.Title = advice.Title.Trim();
                    advice.Description = advice.Description.Trim();
                    advice.CreatedDate = DateTime.Now;

                    db.CareerAdvices.Add(advice);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Career advice posted successfully!";
                    return RedirectToAction("Career", "DashBoard");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while saving. Please try again.";
            }

            return View(advice);
        }
    }
}

