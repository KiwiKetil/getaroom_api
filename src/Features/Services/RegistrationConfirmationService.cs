using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Repositories.Interfaces;
using GetARoomAPI.Features.Services.Interfaces;

namespace GetARoomAPI.Features.Services
{
    public class RegistrationConfirmationService : IRegistrationConfirmationService
    {
        private readonly IRegistrationConfirmationRepository _registrationConfirmationRepository;
        private readonly IEmailSender _emailSender;

        public RegistrationConfirmationService(IRegistrationConfirmationRepository registrationConfirmationRepository, IEmailSender emailSender)
        {
            _registrationConfirmationRepository = registrationConfirmationRepository;
            _emailSender = emailSender;
        }     

        public async Task SendConfirmationEmailAsync(UserId id, string email)
        {
            var token = GenerateConfirmationToken();
            var expiration = DateTime.UtcNow.AddHours(24);

            var tokenEntity = new UserRegistrationToken
            {
                UserId = id,
                TokenString = token,
                DateCreated = DateTime.UtcNow,
                DateExpired = expiration,
                IsConfirmed = false
            };

            await _registrationConfirmationRepository.InsertTokenAsync(tokenEntity);

            var encodedToken = System.Net.WebUtility.UrlEncode(token);
            var confirmationUrl = $"https://localhost:7089/api/v1/users/confirm-registration?token={encodedToken}";
            var subject = "Confirm Your Email Address";
            var message = $"Please confirm your email within 24 hours by clicking the following link: {confirmationUrl}";

            await _emailSender.SendEmailAsync(email, subject, message);
        }

        public string GenerateConfirmationToken()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<bool> ConfirmRegistrationAsync(string token)
        {
            var tokenRecord = await _registrationConfirmationRepository.GetTokenAsync(token);

            if (tokenRecord == null)
            {
                Console.WriteLine("Token record not found for token: {0}", token);
            }
            else
            {
                Console.WriteLine("Retrieved token record: {0}", tokenRecord);
            }

            if (tokenRecord == null ||
                tokenRecord.DateExpired < DateTime.UtcNow ||
                tokenRecord.IsConfirmed)
            {
                return false; // if expired do what?
            }

            tokenRecord.IsConfirmed = true;
            await _registrationConfirmationRepository.UpdateTokenAsync(tokenRecord);
            return true;
        }

        public async Task<bool> HasConfirmedRegistrationAsync(UserId id) 
        {
            var hasConfirmedRegistration = await _registrationConfirmationRepository.HasConfirmedRegistrationAsync(id);
            return hasConfirmedRegistration;
        }
    }
}
