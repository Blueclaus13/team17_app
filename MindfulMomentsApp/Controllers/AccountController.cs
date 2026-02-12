using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using System.Security.Claims;

namespace MindfulMomentsApp.Controllers;

[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AccountController : Controller
{
    public IActionResult Index()
    {
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "User";
        var userPicture = User.FindFirstValue("picture") ?? User.FindFirstValue("urn:google:picture") ?? $"https://ui-avatars.com/api/?name={userName}&background=random";

        var model = new AccountViewModel
        {
            Name = userName,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? "No Email",
            ProfilePictureUrl = userPicture,
            JoinDate = DateTime.Now,
            TotalEntries = 0
        };

        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }


    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> SignIn(string email, string password)
    {
        if (email == "test@gmail.com" && password == "1234")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Email, email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        ViewBag.Error = "Invalid email or password";
        return View();
    }

    [AllowAnonymous]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Account");

        return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl },
            GoogleDefaults.AuthenticationScheme);
    }

    [AllowAnonymous]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return RedirectToAction("SignIn");

        var claims = result.Principal.Claims.ToList();

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