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
public IActionResult Register()
{
    return View();
}
//Register new user with email and password
[AllowAnonymous]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(string email, string password, string firstName, string lastName)
{
    email = email.Trim().ToLowerInvariant();

    var exists = await _context.Users.AnyAsync(u => u.Email.ToLower() == email);
    if (exists)
    {
        ViewBag.Error = "An account with this email already exists.";
        return View();
    }

    var user = new User
    {
        Email = email,
        FirstName = firstName?.Trim() ?? "",
        LastName = lastName?.Trim() ?? "",
       // PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        Password = password, // we will hash this later
        GoogleId = "" // local account
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    // sign user in right away
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Email),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim())
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity));

    return RedirectToAction("Index", "Home");
}


    [AllowAnonymous]
    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }


    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(string email, string password)
    {
        email = email.Trim().ToLowerInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        if (user == null || string.IsNullOrWhiteSpace(user.Password))//user.PasswordHash later
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        var ok = password == user.Password; // BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
        if (!ok)
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        var claims = new List<Claim>
        {
            new  Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
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

        var email = authResult.Principal?.FindFirstValue(ClaimTypes.Email);
        var name = authResult.Principal?.FindFirstValue(ClaimTypes.Name) ?? email ?? "User";
        var picture = authResult.Principal?.FindFirstValue("picture") ?? "";

        if (string.IsNullOrWhiteSpace(email))
            return RedirectToAction("SignIn");

        // OPTIONAL but recommended: ensure user exists in DB
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User
            {
                Email = email,
                FirstName = name,      // you can split later if you want
                LastName = "",
                Password = "",
                GoogleId = authResult.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else
        {
            // keep GoogleId in sync
            var googleSub = authResult.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            if (!string.IsNullOrWhiteSpace(googleSub) && user.GoogleId != googleSub)
            {
                user.GoogleId = googleSub;
                await _context.SaveChangesAsync();
            }
        }

        // IMPORTANT: standardize the cookie identity
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, email), // <-- same as local now
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name)
        };

        if (!string.IsNullOrWhiteSpace(picture))
            claims.Add(new Claim("picture", picture));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("SignIn");
    }
}