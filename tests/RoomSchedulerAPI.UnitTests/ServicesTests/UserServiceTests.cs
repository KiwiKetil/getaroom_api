﻿using AutoFixture.Xunit2;
using AutoMapper;
using GetARoomAPI.Core.DB.UnitOFWork;
using GetARoomAPI.Core.DB.UnitOFWork.Interfaces;
using GetARoomAPI.Features.AutoMapper;
using GetARoomAPI.Features.Models.DTOs.UserDTOs;
using GetARoomAPI.Features.Models.Entities;
using GetARoomAPI.Features.Repositories.Interfaces;
using GetARoomAPI.Features.Services;
using GetARoomAPI.Features.Services.Interfaces;
using GetARoomAPI.UnitTests.CustomAutoDataAttributes;
using Microsoft.Extensions.Logging;
using Moq;

namespace GetARoomAPI.UnitTests.ServicesTests;
public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock = new();
    private readonly Mock<IPasswordVerificationService> _passwordVerificationServiceMock = new();
    private readonly Mock<IPasswordHistoryRepository> _passwordHistoryRepositoryMock = new();
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock = new();
    private readonly Mock<ILogger<UserService>> _loggerMock = new();
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactory = new();

    public UserServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        IMapper mapper = mapperConfig.CreateMapper();

        _userService = new UserService(
            _userRepositoryMock.Object,
            _userRoleRepositoryMock.Object,
            _passwordVerificationServiceMock.Object,
            _passwordHistoryRepositoryMock.Object,
            _tokenGeneratorMock.Object,
            mapper,
            _unitOfWorkFactory.Object,
            _loggerMock.Object
            );
    }

    #region GetUsersAsync

    [Theory]
    [AutoData]
    public async Task GetUsersAsync_ReturnsUsersWithCountDTO(UserQuery userQuery, List<User> users)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUsersAsync(userQuery))
            .ReturnsAsync((users, users.Count));

        //Act
        var result = await _userService.GetUsersAsync(userQuery);

        //Assert
        var res = Assert.IsType<AllUsersDTO>(result);
        Assert.Equal(users.Count, res.TotalCount);
        Assert.Equal(users.Count, res.UserDTOs.Count());
        Assert.Equal(users[0].FirstName, res.UserDTOs.ElementAt(0).FirstName);
        Assert.Equal(users[0].LastName, res.UserDTOs.ElementAt(0).LastName);
        Assert.Equal(users[1].FirstName, res.UserDTOs.ElementAt(1).FirstName);
        Assert.Equal(users[1].LastName, res.UserDTOs.ElementAt(1).LastName);
    }

    [Theory]
    [AutoData]
    public async Task GetUsersAsync_WhenNoUsersAreFound_ReturnsUsersWithCountDTO_WithEmptyCollectionAndTotalCountZero(UserQuery userQuery)
    {
        // Arrange
        List<User> users = [];

        _userRepositoryMock.Setup(x => x.GetUsersAsync(userQuery)).ReturnsAsync((users, users.Count));

        // Act
        var result = await _userService.GetUsersAsync(userQuery);

        // Assert
        var res = Assert.IsType<AllUsersDTO>(result);
        Assert.Empty(res.UserDTOs);
        Assert.Equal(0, res.TotalCount);
    }

    #endregion GetUsersAsync

    #region GetUserByIdAsync

    [Theory]
    [AutoData]
    public async Task GetUserByIdAsync_WhenUserIsFound_ReturnsUserDTO(User user)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(user.Id.Value);

        // Assert
        var res = Assert.IsType<UserDTO>(result);
        Assert.Equal(user.Id, res.Id);
        Assert.Equal(user.FirstName, res.FirstName);
    }

    [Theory]
    [AutoData]
    public async Task GetUserByIdAsync_WhenUserIsNotFound_ReturnsNull(User user)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(user.Id.Value);

        // Assert
        Assert.Null(result);
    }

    #endregion GetUserByIdAsync

    #region UpdateUserAsync

    [Theory]
    [CustomUserAutoData]
    public async Task UpdateUserAsync_WhenUserWasUpdated_ReturnsUserDTO(UserUpdateDTO userUpdateDTO, User user)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.UpdateUserAsync(user.Id, It.IsAny<User>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.UpdateUserAsync(user.Id.Value, userUpdateDTO);

        // Assert
        var res = Assert.IsType<UserDTO>(result);
        Assert.Equal(userUpdateDTO.FirstName, res.FirstName);
        Assert.Equal(userUpdateDTO.LastName, res.LastName);
        Assert.Equal(userUpdateDTO.PhoneNumber, res.PhoneNumber);
        Assert.Equal(userUpdateDTO.Email, res.Email);
    }

    [Theory]
    [CustomUserAutoData]
    public async Task UpdateUserAsync_WhenUserWasNotUpdated_ReturnsNull(UserUpdateDTO userUpdateDTO, User user)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.UpdateUserAsync(user.Id, It.IsAny<User>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateUserAsync(user.Id.Value, userUpdateDTO);

        // Assert
        Assert.Null(result);
    }

    #endregion UpdateUserAsync

    #region DeleteUserAsync

    [Theory]
    [CustomUserAutoData]
    public async Task DeleteUserAsync_WhenUserWasDeleted_ReturnsUserDTO(User user)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.DeleteUserAsync(user.Id))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.DeleteUserAsync(user.Id.Value);

        // Assert
        var res = Assert.IsType<UserDTO>(result);
        Assert.Equal(user.Id, res.Id);
        Assert.Equal(user.FirstName, res.FirstName);
        Assert.Equal(user.LastName, res.LastName);
        Assert.Equal(user.PhoneNumber, res.PhoneNumber);
        Assert.Equal(user.Email, res.Email);
    }

    [Theory]
    [CustomUserAutoData]
    public async Task DeleteUserAsync_WhenUserWasNotDeleted_ReturnsNull(User user)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.DeleteUserAsync(user.Id))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.DeleteUserAsync(user.Id.Value);

        // Assert
        Assert.Null(result);
    }

    #endregion DeleteUserAsync

    #region RegisterUserAsync

    [Theory]
    [AutoData]
    public async Task RegisterUserAsync_WhenUserAlreadyExists_ReturnsNull(ClientRegistrationDTO userRegistrationDTO)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(userRegistrationDTO.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.RegisterUserAsync(userRegistrationDTO);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [CustomUserAutoData]
    public async Task RegisterUserAsync_WhenRegistrationIsSuccessfull_ReturnsUserDTO(ClientRegistrationDTO userRegistrationDTO, User user)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(userRegistrationDTO.Email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock.Setup(x => x.RegisterUserAsync(It.IsAny<User>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.RegisterUserAsync(userRegistrationDTO);

        // Assert
        var res = Assert.IsType<UserDTO>(result);
        Assert.Equal(userRegistrationDTO.FirstName, res.FirstName);
        Assert.Equal(userRegistrationDTO.LastName, res.LastName);
        Assert.Equal(userRegistrationDTO.PhoneNumber, res.PhoneNumber);
        Assert.Equal(userRegistrationDTO.Email, res.Email);
    }

    #endregion RegisterUserAsync

    #region UserLoginAsync

    [Theory]
    [AutoData]
    public async Task UserLoginAsync_WhenUserWasNotFound_ReturnsNull(LoginDTO loginDTO, User user, IEnumerable<UserRole> roles)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.Username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UserLoginAsync(loginDTO);

        // Assert
        Assert.Null(result);
        _passwordVerificationServiceMock.Verify(x => x.VerifyPassword(loginDTO, It.IsAny<User>()), Times.Never);
        _userRoleRepositoryMock.Verify(x => x.GetUserRolesAsync(user.Id), Times.Never);
        _tokenGeneratorMock.Verify(x => x.GenerateToken(user, false, roles), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task UserLoginAsync_WhenPasswordVerificationFails_ReturnsNull(LoginDTO loginDTO, User user, IEnumerable<UserRole> roles)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.Username))
            .ReturnsAsync(user);
        _passwordVerificationServiceMock.Setup(x => x.VerifyPassword(loginDTO, user))
            .Returns(false);

        // Act
        var result = await _userService.UserLoginAsync(loginDTO);

        // Assert
        Assert.Null(result);
        _userRoleRepositoryMock.Verify(x => x.GetUserRolesAsync(user.Id), Times.Never);
        _tokenGeneratorMock.Verify(x => x.GenerateToken(user, false, roles), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task UserLoginAsync_WhenSuccess_ReturnsValidToken(LoginDTO loginDTO, User user, IEnumerable<UserRole> roles)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.Username))
            .ReturnsAsync(user);
        _passwordVerificationServiceMock.Setup(x => x.VerifyPassword(loginDTO, user))
            .Returns(true);
        _userRoleRepositoryMock.Setup(x => x.GetUserRolesAsync(user.Id))
            .ReturnsAsync(roles);
        _tokenGeneratorMock.Setup(x => x.GenerateToken(user, false, roles))
            .Returns("ValidTokenString");

        // Act
        var result = await _userService.UserLoginAsync(loginDTO);

        //Assert
        var res = Assert.IsType<string>(result);
        Assert.Equal("ValidTokenString", res);
    }

    #endregion UserLoginAsync

    #region UpdatePasswordAsync

    [Theory]
    [AutoData]
    public async Task UpdatePasswordAsync_WhenUserWasNotFound_ReturnsNull(
        UpdatePasswordDTO updatePasswordDTO,
        User user,
        IEnumerable<UserRole> roles)
    {
        //Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Username))
            .ReturnsAsync((User?)null);

        //Act
        var result = await _userService.UpdatePasswordAsync(updatePasswordDTO);

        //Assert
        Assert.Null(result);
        _passwordVerificationServiceMock.Verify(x => x.VerifyPassword(updatePasswordDTO, user), Times.Never);
        _userRepositoryMock.Verify(x => x.UpdatePasswordAsync(It.IsAny<IUnitOfWork>(), user.Id, It.IsAny<string>()), Times.Never);
        _userRoleRepositoryMock.Verify(x => x.GetUserRolesAsync(user.Id), Times.Never);
        _tokenGeneratorMock.Verify(x => x.GenerateToken(user, true, roles), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task UpdatePasswordAsync_WhenPasswordVerificationFails_ReturnsNull(
        UpdatePasswordDTO updatePasswordDTO,
        User user,
        IEnumerable<UserRole> roles)
    {
        //Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Username))
            .ReturnsAsync(user);
        _passwordVerificationServiceMock.Setup(x => x.VerifyPassword(updatePasswordDTO, user))
            .Returns(false);

        //Act
        var result = await _userService.UpdatePasswordAsync(updatePasswordDTO);

        //Assert
        Assert.Null(result);
        _userRepositoryMock.Verify(x => x.UpdatePasswordAsync(It.IsAny<IUnitOfWork>(), user.Id, It.IsAny<string>()), Times.Never);
        _userRoleRepositoryMock.Verify(x => x.GetUserRolesAsync(user.Id), Times.Never);
        _tokenGeneratorMock.Verify(x => x.GenerateToken(user, true, roles), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task UpdatePasswordAsync_WhenPasswordUpdateFails_ReturnsNull(
        UpdatePasswordDTO updatePasswordDTO,
        User user,
        IEnumerable<UserRole> roles)
    {
        //Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Username))
            .ReturnsAsync(user);
        _passwordVerificationServiceMock.Setup(x => x.VerifyPassword(updatePasswordDTO, user))
            .Returns(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Id)
            .Returns(new UnitOfWorkId(Guid.NewGuid()));
        unitOfWorkMock.Setup(u => u.BeginAsync())
            .Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.CommitAsync())
            .Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.RollbackAsync())
            .Returns(Task.CompletedTask);
        _unitOfWorkFactory.Setup(f => f.Create())
            .Returns(unitOfWorkMock.Object);

        _userRepositoryMock.Setup(x => x.UpdatePasswordAsync(It.IsAny<IUnitOfWork>(), user.Id, It.IsAny<string>()))
            .ReturnsAsync(false);

        //Act
        var result = await _userService.UpdatePasswordAsync(updatePasswordDTO);

        //Assert
        Assert.Null(result);
        _passwordHistoryRepositoryMock.Verify(x => x.InsertPasswordUpdateRecordAsync(It.IsAny<UnitOFWork>(), user.Id.Value), Times.Never);
        _userRoleRepositoryMock.Verify(x => x.GetUserRolesAsync(user.Id), Times.Never);
        _tokenGeneratorMock.Verify(x => x.GenerateToken(user, true, roles), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task UpdatePasswordAsync_WhenPasswordHistoryRecordUpdateFails_ReturnsNull(
    UpdatePasswordDTO updatePasswordDTO,
    User user,
    IEnumerable<UserRole> roles)
    {
        //Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Username))
            .ReturnsAsync(user);
        _passwordVerificationServiceMock.Setup(x => x.VerifyPassword(updatePasswordDTO, user))
            .Returns(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Id)
            .Returns(new UnitOfWorkId(Guid.NewGuid()));
        unitOfWorkMock.Setup(u => u.BeginAsync())
            .Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.CommitAsync())
            .Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.RollbackAsync())
            .Returns(Task.CompletedTask);
        _unitOfWorkFactory.Setup(f => f.Create())
            .Returns(unitOfWorkMock.Object);

        _userRepositoryMock.Setup(x => x.UpdatePasswordAsync(It.IsAny<IUnitOfWork>(), user.Id, It.IsAny<string>()))
            .ReturnsAsync(true);

        _passwordHistoryRepositoryMock.Setup(x => x.InsertPasswordUpdateRecordAsync(It.IsAny<IUnitOfWork>(), user.Id.Value))
            .ReturnsAsync(false);

        //Act
        var result = await _userService.UpdatePasswordAsync(updatePasswordDTO);

        //Assert
        Assert.Null(result);
        _passwordHistoryRepositoryMock.Verify(x => x.InsertPasswordUpdateRecordAsync(It.IsAny<UnitOFWork>(), user.Id.Value), Times.Never);
        _userRoleRepositoryMock.Verify(x => x.GetUserRolesAsync(user.Id), Times.Never);
        _tokenGeneratorMock.Verify(x => x.GenerateToken(user, true, roles), Times.Never);
    }

    [Theory]
    [AutoData]
    public async Task UpdatePasswordAsync_WhenSuccess_ReturnsTokenWithValidData(UpdatePasswordDTO updatePasswordDTO, User user, IEnumerable<UserRole> roles)
    {
        // Arrange
        _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Username))
            .ReturnsAsync(user);
        _passwordVerificationServiceMock.Setup(x => x.VerifyPassword(updatePasswordDTO, user))
            .Returns(true);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.Id)
            .Returns(new UnitOfWorkId(Guid.NewGuid()));
        unitOfWorkMock.Setup(u => u.BeginAsync())
            .Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.CommitAsync())
            .Returns(Task.CompletedTask);
        unitOfWorkMock.Setup(u => u.RollbackAsync())
            .Returns(Task.CompletedTask);
        _unitOfWorkFactory.Setup(f => f.Create())
            .Returns(unitOfWorkMock.Object);

        _userRepositoryMock.Setup(x => x.UpdatePasswordAsync(It.IsAny<IUnitOfWork>(), user.Id, It.IsAny<string>()))
            .ReturnsAsync(true);
        _passwordHistoryRepositoryMock.Setup(x => x.InsertPasswordUpdateRecordAsync(It.IsAny<IUnitOfWork>(), user.Id.Value))
            .ReturnsAsync(true);

        _userRoleRepositoryMock.Setup(x => x.GetUserRolesAsync(user.Id))
            .ReturnsAsync(roles);
        _tokenGeneratorMock.Setup(x => x.GenerateToken(user, true, roles))
            .Returns("ValidTokenString");

        // Act
        var result = await _userService.UpdatePasswordAsync(updatePasswordDTO);

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("ValidTokenString", result);
    }

    #endregion UpdatePasswordAsync
}
