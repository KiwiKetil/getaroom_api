using RoomSchedulerAPI.Features.Models.DTOs;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IUserAuthenticationService
{
    Task<bool> AuthenticateUserAsync(LoginDTO dto);
}
