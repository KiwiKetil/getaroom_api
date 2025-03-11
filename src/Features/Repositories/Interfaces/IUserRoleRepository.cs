using GetARoomAPI.Core.DB.UnitOFWork.Interfaces;
using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Models.Enums;

namespace GetARoomAPI.Features.Repositories.Interfaces;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetUserRolesAsync(UserId id);
    Task<bool> AddUserRoleAsync(UserId id, UserRoles role, IUnitOfWork unitOfWork);
}
