using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.Features.Services;

public class AuthenticationService(IUserRepository userRepository, ILogger<AuthenticationService> logger) : IUserAuthenticationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<AuthenticationService> _logger = logger;  

    public async Task<User?> AuthenticateUserAsync(LoginDTO dto)
    {
        var user = await _userRepository.GetUserByEmailAsync(dto.Email);

        if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.HashedPassword)) 
        {
            _logger.LogInformation("User Authenticated: {username}", dto.Email);
            return user;
        }
        return null;
    }
}
