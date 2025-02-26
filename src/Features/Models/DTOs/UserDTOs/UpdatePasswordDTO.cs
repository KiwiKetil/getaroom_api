using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;

public record UpdatePasswordDTO : IUserCredentialsDTO
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string NewPassword { get; init; }
}
