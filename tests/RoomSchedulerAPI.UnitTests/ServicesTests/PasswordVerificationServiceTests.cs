using AutoFixture.Xunit2;
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

    #region VerifyPasswordWhenLogin

    [Theory]
    [AutoData]
    public void VerifyPassword_WhenLogin_WhenVerifid_ReturnsTrue(LoginDTO loginDTO, User user)
    {
        // Arrange
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(loginDTO.Password);

        // Act
        var result = _service.VerifyPassword(loginDTO, user);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [AutoData]
    public void VerifyPassword_WhenLogin_WhenVerificationFails_ReturnsFalse(LoginDTO loginDTO, User user)
    {
        // Arrange
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword("TestPassword123!");

        // Act
        var result = _service.VerifyPassword(loginDTO, user);

        // Assert
        Assert.False(result);
    }

    #endregion VerifyPasswordWhenLogin

    #region VerifyPasswordWhenUpdatingPassword

    [Theory]
    [AutoData]
    public void VerifyPassword_WhenUpdatingPassword_WhenVerifid_ReturnsTrue(UpdatePasswordDTO updatePasswordDTO, User user)
    {
        // Arrange
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(updatePasswordDTO.Password);

        // Act
        var result = _service.VerifyPassword(updatePasswordDTO, user);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [AutoData]
    public void VerifyPassword_WhenUpdatingPassword_WhenVerificationFails_ReturnsFalse(UpdatePasswordDTO updatePasswordDTO, User user)
    {
        // Arrange
        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword("CurrentPassword123!");

        // Act
        var result = _service.VerifyPassword(updatePasswordDTO, user);

        // Assert
        Assert.False(result);
    }

    #endregion VerifyPasswordWhenUpdatingPassword
}
