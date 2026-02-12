namespace MindfulMomentsApp.Models
{
  public class User
  {
    public Guid UserId { get; set; } = Guid.NewGuid();
    public string GoogleId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? Photo { get; set; }
  }
}
