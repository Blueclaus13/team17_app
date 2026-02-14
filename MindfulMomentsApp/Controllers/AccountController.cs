using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindfulMomentsApp.Data;
using MindfulMomentsApp.Models;
using System.Security.Claims;
using BCrypt.Net;

namespace MindfulMomentsApp.Controllers;

[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = await _context.Users.Include(u => u.Journal).ThenInclude(j => j!.Entries).FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return RedirectToAction("SignIn");

        var sevenDaysAgo = DateTime.UtcNow.Date.AddDays(-7);
        var weeklyStreak = user.Journal?.Entries?
            .Where(e => e.CreatedDate >= sevenDaysAgo)
            .Select(e => e.CreatedDate.Date)
            .Distinct()
            .Count() ?? 0;

        var userName = $"{user.FirstName} {user.LastName}".Trim();
        if (string.IsNullOrEmpty(userName)) userName = "User";
        var userPicture = User.FindFirstValue("picture") ?? User.FindFirstValue("urn:google:picture") ?? $"https://ui-avatars.com/api/?name={userName}&background=random";
        var model = new AccountViewModel
        {
            Name = userName,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? "No Email",
            ProfilePictureUrl = userPicture,
            JoinDate = DateTime.Now,
            TotalEntries = user.Journal?.Entries?.Count ?? 0,
            WeeklyStreak = weeklyStreak,
            LastEntryDate = user.Journal?.Entries?
                .OrderByDescending(e => e.CreatedDate)
                .FirstOrDefault()?.CreatedDate.ToShortDateString() ?? "No entries yet"
                };

        return View(model);
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
            Password = BCrypt.Net.BCrypt.HashPassword(password), // Hashing password added
            GoogleId = "" // local account
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await SignInUser(user.Email, $"{user.FirstName} {user.LastName}".Trim(), null);

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
        if (user == null || string.IsNullOrEmpty(user.Password) || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        await SignInUser(user.Email, $"{user.FirstName} {user.LastName}".Trim(), null);

        return RedirectToAction("Index", "Home");
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
        var authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
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

            Console.WriteLine($"Attempting to save new user: {email}");
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

        await SignInUser(email, name, picture);

        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("SignIn");
    }

    private async Task SignInUser(string email, string name, string? picture)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name)
        };

        if (!string.IsNullOrWhiteSpace(picture))
            claims.Add(new Claim("picture", picture));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }
}