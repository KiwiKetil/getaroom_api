using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Services.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(User authenticateduser, bool hasChangedPassword, IEnumerable<UserRole> userRoles);
}
