using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Services;

public class AuthenticationService(ILogger<AuthenticationService> logger) : IUserAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger = logger;  

    public bool AuthenticateUser(IAuthorizableDTO dto, User user)    
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
