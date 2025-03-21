using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using RestaurantBusiness.Models;
using RestaurantReservationManagement.Models;

namespace FUCourseManagement.Controllers
{
    public class AuthController : Controller
    {
        private IUserRepository userRepository;

        public AuthController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Please provide email and password.";
                return View();
            }

            var account = await userRepository.Login(model.Email, model.Password);

            if (account != null)
            {
                await SignInAsync(account);
                return Redirect("/Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ViewBag.Error = "Passwords do not match.";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                ViewBag.Error = "Please provide email.";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Please provide password.";
                return View(model);
            }

            User User = new User()
            {
                Email = model.Email,
                Password = model.Password,
                FullName = model.FirstName + " " + model.LastName,
                Role = model.Role.Equals("Admin") ? "Admin" : "Customer",
            };

            var account = await userRepository.Login(User.Email!, User.Password!);
            if (account != null)
            {
                ViewBag.Error = "Email already exists";
                return View(model);
            }
            try
            {
                account = await userRepository.AddAsync(User);
                await SignInAsync(account);
                return Redirect("/Home");
            }
            catch (Exception e)
            {
                ViewBag.Error = "Error creating account: " + e.Message;
                return View(model);
            }
        }

        private async Task SignInAsync(User account)
        {
            var role = account.Role switch
            {
                "Admin" => "Admin",
                "Customer" => "User",
                _ => "User",
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, account.FullName ?? "Unknown"),
                new Claim(ClaimTypes.Email, account.Email ?? ""),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                }
            );
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
