using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulMomentsApp.Data;
using MindfulMomentsApp.Models;
using System.Security.Claims;

namespace MindfulMomentsApp.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "User";
            var userPicture = User.FindFirstValue("picture") ?? $"https://ui-avatars.com/api/?name={userName}&background=random";

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

        return RedirectToAction("SignIn");
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