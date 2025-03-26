namespace GetARoomAPI.Features.Services.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string accessToken, string recipientEmail, string subject, string htmlMessage);
    //Task SendEmailAsync(/*string accessToken, */string recipientEmail, string subject, string htmlMessage); // If using Graph
}
