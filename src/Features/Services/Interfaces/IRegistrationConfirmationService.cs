using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Services.Interfaces;

public interface IRegistrationConfirmationService
{
    string GenerateConfirmationToken();
    Task SendConfirmationEmailAsync(UserId id, string email);
    Task<bool> ConfirmRegistrationAsync(string token);
    Task<bool> HasConfirmedRegistrationAsync(UserId id);

}
