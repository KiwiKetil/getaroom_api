using Microsoft.IdentityModel.Tokens;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoomSchedulerAPI.Features.Services;

public class TokenGenerator(IConfiguration config, ILogger<TokenGenerator> logger) : ITokenGenerator
{
    private readonly IConfiguration _config = config;
    private readonly ILogger _logger = logger;

    public string GenerateToken(User authenticatedUser, bool hasUpdatedPassword, IEnumerable<UserRole> userRoles)
    {
        if (authenticatedUser == null)
        {
            throw new ArgumentException("An authenticated user is needed");
        }

        if (userRoles == null || !userRoles.Any())
        {
            throw new ArgumentException("Role(s) are needed");
        }

        _logger.LogDebug("Generating token for user ID: {UserId}", authenticatedUser.Id);

        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Missing configuration: Jwt:Key");
        var jwtIssuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing configuration: Jwt:Issuer");
        var jwtAudience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Missing configuration: Jwt:Audience");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var userId = authenticatedUser.Id;
        var userName = authenticatedUser.Email;

        List<Claim> claims = [];
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userId.Value.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, userName.ToString()));
        claims.Add(new Claim("passwordUpdated", hasUpdatedPassword ? "true" : "false"));

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
        }

        var token = new JwtSecurityToken
            (
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(240),
            signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
