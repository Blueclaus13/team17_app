using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using System.Security.Claims;

namespace MindfulMomentsApp.Controllers;

public class AccountController : Controller
{
    // YOUR CODE: Displays the Account Page with your data
    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var modelFromGoogle = new AccountViewModel
            {
                Name = User.FindFirstValue(ClaimTypes.Name) ?? "User",
                Email = User.FindFirstValue(ClaimTypes.Email) ?? "No Email",
                ProfilePictureUrl = User.FindFirstValue("picture") ?? "/images/default-avatar.png",
                JoinDate = DateTime.Now,
                TotalEntries = 0
            };

            return View(modelFromGoogle);
        }

        var model = new AccountViewModel
        {
            Name = "Israel Carmona",
            Email = "israel@example.com",
            ProfilePictureUrl = "https://via.placeholder.com/150",
            JoinDate = new DateTime(2026, 1, 10),
            TotalEntries = 14,
            LastEntryDate = "February 3, 2026"
        };
        return View(model);
    }

    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }

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

    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Account");

        return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl },
            GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> GoogleResponse()
    {
        var authResult = await HttpContext.AuthenticateAsync();

        if (!authResult.Succeeded)
            return RedirectToAction("SignIn");

        var claims = authResult.Principal.Claims.ToList();

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("SignIn");
    }
}