using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RoomSchedulerAPI.UnitTests.ServicesTests;
public class TokenGeneratorTests
{
    private readonly TokenGenerator _service;
    private readonly Mock<ILogger<TokenGenerator>> _loggerMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    public TokenGeneratorTests()
    {
        _configMock.Setup(c => c["Jwt:Key"]).Returns("TheSecretKeyIsNowGreaterThan256Bits");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TheIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TheAudience");

        _service = new TokenGenerator(_configMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GenerateToken_WithNullUser_ThrowsArgumentException() 
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => _service.GenerateToken(null!, false, [])); 
    }

    [Fact]
    public void GenerateToken_WhenSuccess_WhenUserHasNotUpdatedPassword_ReturnsValidTokenWithCorrectClaims() 
    {
        // Arrange
        var authenticatedUser = new User { Id = UserId.NewId, Email = "email@123@gmail.com"};
        List<UserRole> roles = [new UserRole { RoleName = "User" }];
        var expectedRoles = roles.Select(r => r.RoleName).ToList();
        var handler = new JwtSecurityTokenHandler();
        var passwordUpdated = false; 

        // Act
        var result = _service.GenerateToken(authenticatedUser, passwordUpdated, roles);

        // Assert
        var jwtToken = handler.ReadJwtToken(result);
        var idClaim = jwtToken.Subject;
        var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name);
        var rolesClaim = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        var passwordUpdatedClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "passwordUpdated");

        Assert.IsType<string>(result);        
        Assert.NotNull(nameClaim);
        Assert.NotNull(passwordUpdatedClaim);
        Assert.Equal(authenticatedUser.Id.Value.ToString(), idClaim);
        Assert.Equal(authenticatedUser.Email, nameClaim.Value);
        Assert.Equal(expectedRoles, rolesClaim);
        Assert.False(bool.Parse(passwordUpdatedClaim.Value));
    }

    [Fact]
    public void GenerateToken_WhenSuccess_WhenUserHasUpdatedPassword_ReturnsValidTokenWithCorrectClaims()
    {
        // Arrange
        var authenticatedUser = new User { Id = UserId.NewId, Email = "email@123@gmail.com" };

        // Act
        var result = _service.GenerateToken(authenticatedUser, true, []);

        // Assert
        Assert.IsType<string>(result);

    }

    // when userroles is empty => no roles
    // when userroles has roles, is in token?

}

