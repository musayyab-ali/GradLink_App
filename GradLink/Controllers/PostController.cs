using GradLink.Repository.MSSQL.ORM.Context;
using GradLink.Repository.MSSQL.ORM.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace GradLink.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly GradLinkDbContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostController(GradLinkDbContext dbContext, IHttpContextAccessor httpContextAccessor)
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

        //public IActionResult Index()
        //{
        //    var posts = db.Posts
        // .Include("User")
        // .Include("Likes")
        // .Include("Comments.User")
        // .OrderByDescending(p => p.CreatedOn)
        // .ToList();
        //    ViewBag.UserName = User?.Identity?.Name ?? "Guest";
        //    return View(posts);
        //}

        public IActionResult Index()
        {
            var posts = db.Posts
                .Include("User")
                .Include("Likes")
                .Include("Comments.User")
                .OrderByDescending(p => p.CreatedOn)
                .ToList();

            return View(posts);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost(IFormFile imageFile, string textContent)
        {
            // Get the UserID from the current logged-in user
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Path for the uploaded image
            string imagePath = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Path.GetFileName(imageFile.FileName);
                var serverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Content", "img", "Posts", fileName);

                // Save the uploaded file to the server
                using (var stream = new FileStream(serverPath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                imagePath = "/Content/img/Posts/" + fileName;
            }

            // Create the post object
            var post = new Post
            {
                TextContent = textContent,
                ImagePath = imagePath,
                UserId = Convert.ToInt32(userId),  // Assuming your user ID is an integer
                CreatedOn = DateTime.Now
            };

            // Add the post to the database
            db.Posts.Add(post);
            db.SaveChanges();

            // Add success message
            TempData["SuccessMessage"] = "Post Added successfully!";

            // Redirect to the Index action
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int postId, string commentText)
        {
            //string userId = User.Identity.GetUserId();
            var comment = new Comment
            {
                PostId = postId,
                CommentText = commentText,
                UserId = (int)UserID,//Convert.ToInt32(userId),
                CreatedOn = DateTime.Now
            };

            db.Comments.Add(comment);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Add Comment successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ToggleLike(int postId)
        {
            //int userId = Convert.ToInt32(User.Identity.GetUserId());
            var existingLike = db.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == UserID);

            if (existingLike != null)
            {
                db.Likes.Remove(existingLike);
                db.SaveChanges();

                int updatedLikeCount = db.Likes.Count(l => l.PostId == postId);

                return Json(new { success = true, likeCount = updatedLikeCount, message = "You have unliked the post." });
            }
            else
            {
                // Like the post
                db.Likes.Add(new Like
                {
                    PostId = postId,
                    UserId = (int)UserID,
                    CreatedOn = DateTime.Now
                });

                db.SaveChanges();

                // Get the updated like count
                int updatedLikeCount = db.Likes.Count(l => l.PostId == postId);

                return Json(new { success = true, likeCount = updatedLikeCount, message = "Thanks for liking the post!" });
            }
        }

        [HttpGet]
        public JsonResult GetLikedUsers(int postId)
        {
            var likedUsers = db.Likes
                .Where(l => l.PostId == postId)
                .Select(l => new
                {
                    l.User.UserId,
                    l.User.Username,
                    l.User.Email // optional
                })
                .ToList();

            return Json(likedUsers);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int postId)
        {
            var post = db.Posts.Include("Comments").FirstOrDefault(p => p.PostId == postId);
            if (post != null)
            {
                db.Comments.RemoveRange(post.Comments);
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Post deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Post not found.";
            }

            return RedirectToAction("Index");
        }
    }
}
