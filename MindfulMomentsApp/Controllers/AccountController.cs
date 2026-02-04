using Microsoft.AspNetCore.Mvc;
using MindfulMomentsApp.Models;
using System.Security.Claims;

namespace MindfulMomentsApp.Controllers;

public class AccountController : Controller
{
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
}