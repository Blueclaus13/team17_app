using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MindfulMomentsApp.Controllers
{
    public class AccountController : Controller
    {
        // Landing page for login form
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        // Local login (temporary, no database)
        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            if (email == "test@gmail.com" && password == "1234")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        // Google Login
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");

            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl },
                GoogleDefaults.AuthenticationScheme);
        }

        // Google Callback
        public async Task<IActionResult> GoogleResponse()
        {
            var authResult = await HttpContext.AuthenticateAsync();

            if (!authResult.Succeeded)
                return RedirectToAction("SignIn");

            // Extract Google user information
            var claims = authResult.Principal.Claims.ToList();

            // Create app authentication cookie
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn");
        }
    }
}
