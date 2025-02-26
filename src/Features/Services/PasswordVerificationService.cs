using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Services;

public class PasswordVerificationService(ILogger<PasswordVerificationService> logger) : IPasswordVerificationService
{
    private readonly ILogger<PasswordVerificationService> _logger = logger;

    public bool VerifyPassword(IUserCredentialsDTO dto, User user)
    {
        var verified = BCrypt.Net.BCrypt.Verify(dto.Password, user.HashedPassword);
        if (!verified)
        {
            _logger.LogDebug("Invalid current password");
            return false;
        }
        return verified;
    }
}
