using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(User authenticateduser, bool hasChangedPassword, IEnumerable<UserRole> userRoles);
}
