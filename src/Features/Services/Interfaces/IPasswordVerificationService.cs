using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IPasswordVerificationService
{
    bool VerifyPassword(IUserCredentialsDTO dto, User user);
}
