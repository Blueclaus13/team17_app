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

/// <summary>
/// Handles user authentication and account management for both local and Google OAuth accounts.
/// </summary>
[Authorize]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays the user account dashboard with profile information and journal statistics.
    /// </summary>
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

    /// <summary>
    /// Displays the registration form for creating a new local account.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    /// <summary>
    /// Processes new user registration, hashes password with BCrypt, and automatically signs in the user.
    /// </summary>
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
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            GoogleId = ""
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await SignInUser(user.Email, $"{user.FirstName} {user.LastName}".Trim(), null);

        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Displays the sign-in form for existing users.
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    public IActionResult SignIn()
    {
        return View();
    }

    /// <summary>
    /// Authenticates user credentials using BCrypt password verification and creates authentication cookie.
    /// </summary>
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

    /// <summary>
    /// Initiates the Google OAuth authentication flow.
    /// </summary>
    [AllowAnonymous]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Account");

        return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl },
            GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Handles Google OAuth callback, creates or updates user in database, and signs them in.
    /// </summary>
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

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {

            Console.WriteLine($"Attempting to save new user: {email}");
            user = new User
            {
                Email = email,
                FirstName = name,
                LastName = "",
                Password = "",
                GoogleId = authResult.Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        else
        {
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

    /// <summary>
    /// Signs out the current user and redirects to the sign-in page.
    /// </summary>
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("SignIn");
    }

    /// <summary>
    /// Creates standardized authentication cookie using email as the primary identifier for both local and Google accounts.
    /// </summary>
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