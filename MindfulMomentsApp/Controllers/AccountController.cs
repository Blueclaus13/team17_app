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
            await EnsureUserAndJournalAsync(email, email, email, null, null);

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

        var externalId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                         ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                         ?? string.Empty;

        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
        var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
        var picture = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

        if (!string.IsNullOrEmpty(externalId) && !string.IsNullOrEmpty(email))
        {
            await EnsureUserAndJournalAsync(externalId, email, firstName, lastName, picture);
        }

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

    private async Task<User> EnsureUserAndJournalAsync(
        string externalId,
        string email,
        string? firstName,
        string? lastName,
        string? photoUrl)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.GoogleId == externalId);

        if (user == null)
        {
            user = new User
            {
                GoogleId = externalId,
                Email = email,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty,
                Photo = photoUrl
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        var journal = await _context.Journals.SingleOrDefaultAsync(j => j.UserId == user.UserId);
        if (journal == null)
        {
            var journalName = string.IsNullOrWhiteSpace(user.FirstName)
                ? "My Journal"
                : $"{user.FirstName}'s Journal";

            journal = new Journal
            {
                UserId = user.UserId,
                JournalName = journalName
            };

            _context.Journals.Add(journal);
            await _context.SaveChangesAsync();
        }

        return user;
    }
}