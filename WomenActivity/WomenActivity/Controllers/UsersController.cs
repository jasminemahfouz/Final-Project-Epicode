using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WomenActivity.Models;

namespace WomenActivity.Controllers
{
    public class UsersController : Controller
    {
        private readonly WomenActivityDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(WomenActivityDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Users/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                ModelState.AddModelError(string.Empty, "Username already exists.");
                return View(user);
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userProfile = new UserProfile
            {
                UserId = user.Id,
                Username = user.Username,
                Email = "",
                Age = 0,
                Location = ""
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // GET: Users/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        public async Task<IActionResult> Login(User loginUser)
        {
            if (!ModelState.IsValid)
            {
                return View(loginUser);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(loginUser);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "MainPage");
        }

        // POST: Users/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        
    }
}