namespace GetARoomAPI.Features.Models.Entities;

public class UserRegistrationToken
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public UserId UserId { get; set; }
    public string? TokenString { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateExpired { get; set; }
    public bool IsConfirmed { get; set; }
}
