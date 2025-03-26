using GetARoomAPI.Features.Services.Interfaces;
using RestSharp;
using RestSharp.Authenticators;

namespace GetARoomAPI.Features.Services;

public class MailgunEmailSender : IEmailSender
{
    public async Task<RestResponse> SendEmailAsync()
    {
        // Set up the RestClient with the Mailgun API base URL and Basic Authentication.
        var options = new RestClientOptions("https://api.mailgun.net/v3")
        {
            // The username is "api" and the password is your API key.
            // This code checks for an environment variable "API_KEY" and falls back to "API_KEY" if not found.
            Authenticator = new HttpBasicAuthenticator("api", Environment.GetEnvironmentVariable("API_KEY") ?? "API_KEY")
        };

        var client = new RestClient(options);

        // The endpoint includes your sandbox domain.
        var request = new RestRequest("/sandboxb338dc2853664d609b7a2980a5e25fd7.mailgun.org/messages", Method.Post);

        // We indicate that the request body will be sent as multipart form data.
        request.AlwaysMultipartFormData = true;

        // Adding parameters required by Mailgun:
        // "from" is the sender address. For sandbox, it's usually provided by Mailgun.
        request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxb338dc2853664d609b7a2980a5e25fd7.mailgun.org>");
        // "to" is the recipient's email address.
        request.AddParameter("to", "ketil sveberg <ketilsveberg@gmail.com>");
        // "subject" is the subject line.
        request.AddParameter("subject", "Hello ketil sveberg");
        // "text" is the plain text content of the email.
        request.AddParameter("text", "Congratulations ketil sveberg, you just sent an email with Mailgun! You are truly awesome!");

        // Execute the request asynchronously and return the response.
        var response = await client.ExecuteAsync(request);
        Console.WriteLine($"Status: {response.StatusCode}");
        Console.WriteLine($"Content: {response.Content}");
        return response;
    }
}
