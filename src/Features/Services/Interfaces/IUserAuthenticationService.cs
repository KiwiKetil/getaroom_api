using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IUserAuthenticationService
{
    bool AuthenticateUser(IAuthorizableDTO dto, User user);
}
