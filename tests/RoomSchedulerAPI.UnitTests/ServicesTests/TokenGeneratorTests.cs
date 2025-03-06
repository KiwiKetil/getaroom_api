using AutoFixture.Xunit2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
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
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TheIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TheAudience");
        _configMock.Setup(c => c["Jwt:Key"]).Returns("TheSecretKeyIsNowGreaterThan256Bits");

        _service = new TokenGenerator(_configMock.Object, _loggerMock.Object);
    }

    [Theory]
    [AutoData]
    public void GenerateToken_WhenUserIsNull_ThrowsArgumentException(List<UserRole> roles)
    {
        // Arrange, Act & Assert
        var res = Assert.Throws<ArgumentException>(() => _service.GenerateToken(null!, false, roles));
        Assert.Equal("An authenticated user is needed", res.Message);
    }

    [Theory]
    [AutoData]
    public void GenerateToken_WhenRolesAreNullOrEmpty_ThrowsArgumentException(User user)
    {
        // Arrange, Act & Assert
        var res = Assert.Throws<ArgumentException>(() => _service.GenerateToken(user, false, []));
        Assert.Equal("Role(s) are needed", res.Message);
    }

    [Theory]
    [AutoData]
    public void GenerateToken_WhenJwtKeyIsNotSet_ThrowsInvalidOperationException(User user, List<UserRole> roles)
    {
        // Arrange
        _configMock.Setup(c => c["Jwt:Key"]).Returns((string?)null);

        // Act & Assert
        var res = Assert.Throws<InvalidOperationException>(() => _service.GenerateToken(user, false, roles));
        Assert.Equal("Missing configuration: Jwt:Key", res.Message);
    }

    [Theory]
    [AutoData]
    public void GenerateToken_WhenJwtIssuerIsNotSet_ThrowsInvalidOperationException(User user, List<UserRole> roles)
    {
        // Arrange
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns((string?)null);

        // Act & Assert
        var res = Assert.Throws<InvalidOperationException>(() => _service.GenerateToken(user, false, roles));
        Assert.Equal("Missing configuration: Jwt:Issuer", res.Message);
    }

    [Theory]
    [AutoData]
    public void GenerateToken_WhenJwtAudienceIsNotSet_ThrowsInvalidOperationException(User user, List<UserRole> roles)
    {
        // Arrange
        _configMock.Setup(c => c["Jwt:Audience"]).Returns((string?)null);

        // Act & Assert
        var res = Assert.Throws<InvalidOperationException>(() => _service.GenerateToken(user, false, roles));
        Assert.Equal("Missing configuration: Jwt:Audience", res.Message);
    }

    [Theory]
    [AutoData]
    public void GenerateToken_WhenSuccess_WhenUserHasNotUpdatedPassword_ReturnsValidTokenWithCorrectClaims(User user, List<UserRole> roles)
    {
        // Arrange
        var expectedRoles = roles.Select(r => r.RoleName).ToList();

        // Act
        var result = _service.GenerateToken(user, false, roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(result);
        var idClaim = jwtToken.Subject;
        var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
        var rolesClaim = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        var passwordUpdatedClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "passwordUpdated");

        Assert.IsType<string>(result);
        Assert.Equal(user.Id.Value.ToString(), idClaim);
        Assert.NotNull(nameClaim);
        Assert.Equal(user.Email, nameClaim.Value);
        Assert.Equal(expectedRoles, rolesClaim);
        Assert.Contains("TheIssuer", jwtToken.Issuer);
        Assert.Contains("TheAudience", jwtToken.Audiences);
        Assert.NotNull(passwordUpdatedClaim);
        Assert.False(bool.Parse(passwordUpdatedClaim.Value));
    }

    [Theory]
    [AutoData]
    public void GenerateToken_WhenSuccess_WhenUserHasUpdatedPassword_ReturnsValidTokenWithCorrectClaims(User user, List<UserRole> roles)
    {
        // Arrange
        var expectedRoles = roles.Select(r => r.RoleName).ToList();

        // Act
        var result = _service.GenerateToken(user, true, roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(result);
        var idClaim = jwtToken.Subject;
        var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);
        var rolesClaim = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(r => r.Value).ToList();
        var rolesOutput = string.Join(", ", rolesClaim);

        var passwordUpdatedClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "passwordUpdated");

        Assert.IsType<string>(result);
        Assert.Equal(user.Id.Value.ToString(), idClaim);
        Assert.NotNull(nameClaim);
        Assert.Equal(user.Email, nameClaim.Value);
        Assert.Equal(expectedRoles, rolesClaim);
        Assert.Contains("TheIssuer", jwtToken.Issuer);
        Assert.Contains("TheAudience", jwtToken.Audiences);
        Assert.NotNull(passwordUpdatedClaim);
        Assert.True(bool.Parse(passwordUpdatedClaim.Value));
    }
}
