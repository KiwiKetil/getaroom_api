using GetARoomAPI.Features.Services.Interfaces;

namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record UpdatePasswordDTO : IVerifyUserCredentials
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string NewPassword { get; init; }
}
