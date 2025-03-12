using GetARoomAPI.Features.Models.DTOs.UserDTOs;
using GetARoomAPI.Features.Models.Enums;
using System.Security.Claims;

namespace GetARoomAPI.Features.Services.Interfaces;

public interface IUserService
{
    Task<UsersWithCountDTO> GetUsersAsync(UserQuery query);
    Task<UserDTO?> GetUserByIdAsync(Guid id);
    Task<UserDTO?> UpdateUserAsync(Guid id, UserUpdateDTO dto);
    Task<UserDTO?> DeleteUserAsync(Guid id);
    Task<UserDTO?> RegisterUserAsync(UserRegistrationDTO dto, UserRoles role);
    Task<string?> UserLoginAsync(LoginDTO dto);
    Task<string?> UpdatePasswordAsync(UpdatePasswordDTO dto);
}