/*using GetARoomAPI.Features.Services.Interfaces;
using System.Net.Http.Headers;

namespace GetARoomAPI.Features.Services;

public class MailgunEmailSender : IEmailSender
{
    // Replace these with your actual Mailgun details.
    private readonly string _mailgunDomain = "YOUR_DOMAIN_NAME"; // e.g., mg.yourdomain.com
    private readonly string _mailgunApiKey = "YOUR_MAILGUN_API_KEY";
    private readonly string _fromEmail = "no-reply@yourdomain.com";
    private readonly string _fromName = "Your App Name";

    public async Task SendEmailAsync(string recipientEmail, string subject, string htmlMessage)
    {
        using (var client = new HttpClient())
        {
            // Mailgun requires HTTP Basic Auth with username "api" and the API key as the password.
            var authToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"api:{_mailgunApiKey}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            // Mailgun endpoint for sending messages.
            var endpoint = $"https://api.mailgun.net/v3/{_mailgunDomain}/messages";

            // Build form content for the email.
            var content = new MultipartFormDataContent
            {
                { new StringContent($"{_fromName} <{_fromEmail}>"), "from" },
                { new StringContent(recipientEmail), "to" },
                { new StringContent(subject), "subject" },
                { new StringContent(htmlMessage), "html" }
            };

            // Send the POST request.
            var response = await client.PostAsync(endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error sending email via Mailgun: {response.StatusCode} - {errorContent}");
            }
        }
    }
}
*/
