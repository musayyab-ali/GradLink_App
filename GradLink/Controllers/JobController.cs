using GradLink.Model.ViewModel.Job;
using GradLink.Repository.MSSQL.ORM.Context;
using GradLink.Repository.MSSQL.ORM.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GradLink.Controllers
{
    public class JobController : Controller
    {
        private readonly GradLinkDbContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobController(GradLinkDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public long UserID
        {
            get
            {
                // Retrieve the NameIdentifier claim from the current user
                var userIdClaim = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Try parsing the claim as a long and return the parsed value or 0 if parsing fails
                return long.TryParse(userIdClaim, out long userID) ? userID : 0;
            }
        }

        public IActionResult Index()
        {
            //var userId = Convert.ToInt32(User.Identity.GetUserId());

            var jobPosts = db.Jobs.OrderByDescending(j => j.PostedDate).ToList();
            var appliedJobIds = db.JobApplications
                .Where(a => a.UserId == UserID)
                .Select(a => a.JobId ?? 0)
                .ToList();

            var vm = new JobApplicationViewModel
            {
                JobPosts = jobPosts,
                AppliedJobIds = appliedJobIds
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyJob(int jobId, string toEmail, IFormFile resumeFile)
        {
            var userId = User.Identity.Name; // Use User.Identity for user details
            string cvPath = null;

            // Check if a file is uploaded and its content length is greater than 0
            if (resumeFile != null && resumeFile.Length > 0)
            {
                // Get the file name and determine the save path
                var fileName = Path.GetFileName(resumeFile.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Content", "CVs", fileName);

                // Ensure the directory exists
                var directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Save the uploaded file to the path
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    resumeFile.CopyTo(stream);
                }

                // Store the relative path of the uploaded CV
                cvPath = "/Content/CVs/" + fileName;
            }

            // Create a new job application record
            var application = new JobApplication
            {
                JobId = jobId,
                UserId = (int?)UserID,
                Email = toEmail,
                CvPath = cvPath,
                AppliedOn = DateTime.Now
            };

            // Add the application to the database
            db.JobApplications.Add(application);
            db.SaveChanges();

            // Show success message and redirect
            TempData["SuccessMessage"] = "Job applied successfully!";
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Job job)
        {
            if (ModelState.IsValid)
            {
                job.PostedDate = DateTime.Now;
                job.Status = 1;
                db.Jobs.Add(job);
                db.SaveChanges();
                TempData["SuccessMessage"] = "New job created successfully!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "Unwanted error occured to create job.";
            return RedirectToAction("Index");
        }
    }
}
