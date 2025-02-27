using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Services;

namespace RoomSchedulerAPI.UnitTests.ServicesTests;
public class TokenGeneratorTests
{
    private readonly TokenGenerator _service;
    private readonly Mock<ILogger<TokenGenerator>> _loggerMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    public TokenGeneratorTests()
    {
        _configMock.Setup(c => c["Jwt:Key"]).Returns("TheSecretKey");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TheIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TheAudience");

        _service = new TokenGenerator(_configMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void GenerateToken_WithNullUser_ThrowsArgumentException() 
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => _service.GenerateToken(null!, false, [])); ;
    }


}
