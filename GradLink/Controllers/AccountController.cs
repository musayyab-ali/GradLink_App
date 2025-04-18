using GradLink.Model.ViewModel.Account;
using GradLink.Repository.MSSQL.ORM.Context;
using GradLink.Repository.MSSQL.ORM.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;

namespace GradLink.Controllers
{
    public class AccountController : Controller
    {
        private readonly GradLinkDbContext _dbContext;
        public AccountController(GradLinkDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Post");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    // Retrieve the role for the user
                    var userRole = (from ur in _dbContext.UserRoles
                                    join r in _dbContext.Roles on ur.RoleId equals r.RoleId
                                    where ur.UserId == user.UserId
                                    select r.RoleName).FirstOrDefault() ?? "User";

                    // Create claims for the user
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, userRole)
                    };

                    // Create an identity and set the authentication cookie
                    var identity = new ClaimsIdentity(claims, "ApplicationCookie");
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync("ApplicationCookie", principal);

                    TempData["SuccessMessage"] = "Login successful!";
                    return RedirectToAction("Index", "Post");
                }

                TempData["ErrorMessage"] = "Invalid email or password.";
            }
            else
            {
                TempData["ErrorMessage"] = "Please check the form for errors.";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var exists = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);

                if (exists != null)
                {
                    TempData["ErrorMessage"] = "Email already exists.";
                    return RedirectToAction("Register");
                }

                var user = new User
                {
                    Username = model.UserName,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    CreatedDate = DateTime.Now
                };

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                // Assign the default role to the user
                var defaultRole = _dbContext.Roles.FirstOrDefault(r => r.RoleName == "User");
                if (defaultRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = defaultRole.RoleId
                    };
                    _dbContext.UserRoles.Add(userRole);
                    _dbContext.SaveChanges();
                }

                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync("ApplicationCookie");
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _dbContext.Users.Find(userId);

            if (user == null)
                return NotFound();

            var model = new UserProfileViewModel
            {
                UserName = user.Username,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Address = user.Address
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = _dbContext.Users.Find(userId);

                if (user == null)
                    return NotFound();

                user.Username = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.Phone;
                user.Address = model.Address;

                _dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            return View(model);
        }
    }
}

