using Microsoft.Extensions.Logging;
using Moq;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Services;

namespace RoomSchedulerAPI.UnitTests.ServicesTests;
public class PasswordVerificationServiceTests
{
    private readonly PasswordVerificationService _service;
    private readonly Mock<ILogger<PasswordVerificationService>> _loggerMock = new();

    public PasswordVerificationServiceTests()
    {
        _service = new PasswordVerificationService(_loggerMock.Object); 
    }

    [Fact]
    public void VerifyPassword_WhenLogin_WhenVerifid_ReturnsTrue()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CurrentPassword123!");

        var loginDTO = new LoginDTO
        {
            Email = "aaa@emailstest.com",
            Password = "CurrentPassword123!",
        };

        var user = new User { HashedPassword = hashedPassword };

        // Act
        var result = _service.VerifyPassword(loginDTO, user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WhenLogin_WhenVerificationFails_ReturnsFalse()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CurrentPassword123!");

        var loginDTO = new LoginDTO
        {
            Email = "aaa@emailstest.com",
            Password = "WrongPassword123!",
        };

        var user = new User { HashedPassword = hashedPassword };

        // Act
        var result = _service.VerifyPassword(loginDTO, user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WhenUpdatingPassword_WhenVerifid_ReturnsTrue()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CurrentPassword123!");

        var updatePasswordDTO = new UpdatePasswordDTO
        {
            Email = "aaa@emailstest.com",
            Password = "CurrentPassword123!",
            NewPassword = "TheNewPassword123!"
        };

        var user = new User { HashedPassword = hashedPassword };

        // Act
        var result = _service.VerifyPassword(updatePasswordDTO, user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WhenUpdatingPassword_WhenVerificationFails_ReturnsFalse()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CurrentPassword123!");

        var updatePasswordDTO = new UpdatePasswordDTO
        {
            Email = "aaa@emailstest.com",
            Password = "WrongPassword123!",
            NewPassword = "TheNewPassword123!"
        };

        var user = new User { HashedPassword = hashedPassword };

        // Act
        var result = _service.VerifyPassword(updatePasswordDTO, user);

        // Assert
        Assert.False(result);
    }
}
