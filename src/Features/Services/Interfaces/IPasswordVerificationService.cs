using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Services.Interfaces;

public interface IPasswordVerificationService
{
    bool VerifyPassword(IVerifyUserCredentials dto, User user);
}
