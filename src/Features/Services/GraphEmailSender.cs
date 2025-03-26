/*using GetARoomAPI.Features.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class GraphEmailSender : IEmailSender // Not used as I could not use Graph as I don not own an email with Exchange Online Capabilities / fully provisioned mailbox (school/organization))
{
    public async Task SendEmailAsync(string accessToken, string recipientEmail, string subject, string htmlMessage)
    {
        // Create an HttpClient instance
        using (HttpClient client = new HttpClient())
        {
            // Add the access token to the Authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Define the Microsoft Graph endpoint for sending mail
            var endpoint = "https://graph.microsoft.com/v1.0/users/apitester999@outlook.com/sendMail";

            // Build the email message payload
            var emailPayload = new
            {
                message = new
                {
                    subject = subject,
                    body = new
                    {
                        contentType = "HTML",
                        content = htmlMessage
                    },
                    toRecipients = new[]
                    {
                    new { emailAddress = new { address = recipientEmail } }
                }
                },
                saveToSentItems = true
            };

            // Serialize the payload to JSON
            var jsonPayload = JsonSerializer.Serialize(emailPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await client.PostAsync(endpoint, content);

            // Check the response for debugging
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error sending email via Graph API: {response.ReasonPhrase} - {errorContent}");
            }
        }
    }
}
*/