using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Services.Interfaces;

public interface ITokenGenerator
{
    string GenerateToken(User authenticateduser, bool hasConfirmedRegistration, IEnumerable<UserRole> userRoles);
}
