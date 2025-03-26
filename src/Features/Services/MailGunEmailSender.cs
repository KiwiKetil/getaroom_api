using GetARoomAPI.Features.Services.Interfaces;
using RestSharp;
using RestSharp.Authenticators;

namespace GetARoomAPI.Features.Services;

public class MailgunEmailSender : IEmailSender
{
    public async Task<RestResponse> SendEmailAsync(string email, string subject, string message)
    {
        var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "API_KEY";

        var options = new RestClientOptions("https://api.mailgun.net/v3")
        {            
            Authenticator = new HttpBasicAuthenticator("api", apiKey)
        };

        var client = new RestClient(options);

        var request = new RestRequest("/sandboxb338dc2853664d609b7a2980a5e25fd7.mailgun.org/messages", Method.Post)
        {
            AlwaysMultipartFormData = true
        };

        request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxb338dc2853664d609b7a2980a5e25fd7.mailgun.org>");
        request.AddParameter("to", email);
        request.AddParameter("subject", subject);
        request.AddParameter("text", message);

        return await client.ExecuteAsync(request);       
    }
}
