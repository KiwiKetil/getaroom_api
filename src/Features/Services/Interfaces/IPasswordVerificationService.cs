using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Services.Interfaces;

public interface IPasswordVerificationService
{
    bool VerifyPassword(IVerifyUserCredentials dto, User user);
}
