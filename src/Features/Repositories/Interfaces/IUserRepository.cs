using GetARoomAPI.Core.DB.UnitOFWork.Interfaces;
using GetARoomAPI.Features.Models.DTOs.UserDTOs;
using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Repositories.Interfaces;

public interface IUserRepository
{
    Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(UserQuery query);
    Task<User?> GetUserByIdAsync(UserId Id);
    Task<User?> UpdateUserAsync(UserId id, User user);
    Task<User?> DeleteUserAsync(UserId id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> RegisterUserAsync(User user);
    Task<bool> UpdatePasswordAsync(IUnitOfWork uow, UserId id, string newHashedPassword);
}
