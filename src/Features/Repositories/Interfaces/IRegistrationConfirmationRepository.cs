using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Repositories.Interfaces
{
    public interface IRegistrationConfirmationRepository
    {
        Task InsertTokenAsync(UserRegistrationToken token);
        Task<UserRegistrationToken?> GetTokenAsync(string tokenString);
        Task UpdateTokenAsync(UserRegistrationToken token);
    }
}
