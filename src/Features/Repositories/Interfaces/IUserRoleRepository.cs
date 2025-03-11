using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Repositories.Interfaces;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetUserRolesAsync(UserId id);
}
