
namespace RoomSchedulerAPI.Features.Models.Entities;

public readonly record struct UserId(Guid userId)
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
    public string? Email { get; set; }
    public string? HashedPassword { get; set; }
    public string? Salt { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}