using Microsoft.AspNetCore.SignalR;

namespace MindfulMomentsApp.Models;

public class AccountViewModel
{
  // Basic User Info from Google OAuth
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string ProfilePictureUrl { get; set; } = "images/default-avatar.png";

  // Journaling Stats
  public DateTime JoinDate { get; set; }
  public int TotalEntries { get; set; }
  public string LastEntryDate { get; set; } = "No entries yet";

}