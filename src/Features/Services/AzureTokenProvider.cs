/*using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

public class AzureTokenProvider // Not used since I could not use Graph as I dont have an organtization/schoool email with Exchange Online Capabilities / fully provisioned mailbox
{
    // Replace with your actual values from your app registration
    private const string ClientId = "5cd665c1-7143-44cb-9ad4-4b3e05aee5e8";
    private const string TenantId = "06994ccb-311a-4b9d-a24e-7b74d58ac2c5";
    private const string ClientSecret = // The secret you created

    // The client credentials flow uses the .default scope to request all permissions granted in the app registration.
    private readonly string[] Scopes = new string[] { "https://graph.microsoft.com/.default" };

    public async Task<string> GetAccessTokenAsync()
    {
        // Build the confidential client application
        IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
            .Create(ClientId)
            .WithClientSecret(ClientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{TenantId}"))
            .Build();

        // Acquire token using client credentials (non-interactive)
        var authResult = await app.AcquireTokenForClient(Scopes).ExecuteAsync();

        // Optionally, decode and print token claims if it's a JWT.
        var tokenHandler = new JwtSecurityTokenHandler();
        if (tokenHandler.CanReadToken(authResult.AccessToken))
        {
            var jwtToken = tokenHandler.ReadJwtToken(authResult.AccessToken);
            Console.WriteLine("JWT Claims:");
            foreach (var claim in jwtToken.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }
        }
        else
        {
            Console.WriteLine("Token is opaque and cannot be decoded.");
        }

        return authResult.AccessToken;
    }
}
*/
