
namespace RoomSchedulerAPI.Features.Models.Entities;

public readonly record struct UserId(Guid Value)
{
    public static UserId NewId => new(Guid.NewGuid());
    public static UserId Empty => new(Guid.Empty);
};

public class User
{
    public UserId Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? HashedPassword { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}