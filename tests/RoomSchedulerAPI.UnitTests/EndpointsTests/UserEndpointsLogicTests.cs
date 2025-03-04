using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using RoomSchedulerAPI.Core.Authorization;
using RoomSchedulerAPI.Features.Endpoints.Logic;
using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.DTOs.ResponseDTOs;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Security.Claims;

namespace RoomSchedulerAPI.UnitTests.EndpointsTests;
public class UserEndpointsLogicTests
{
    private readonly Mock<IUserService> _userServiceMock = new();
    private readonly Mock<IAuthorizationService> _authorizationServiceMock = new();
    private readonly Mock<ILogger<Program>> _loggerMock = new();

    #region GetUsers

    [Fact]
    public async Task GetUsersLogicAsync_WhenUsersExist_ReturnsOkWithValidData()
    {
        // Arrange
        var query = new UserQuery(null, null, null, null);
        var users = new List<UserDTO>();
        var links = new List<Link>();

        var userDTOs = new List<UserDTO>
        {
            new(UserId.NewId, "Ketil", "Sveberg", "91914455", "ketilsveberg@gmail.com", links),
            new(UserId.NewId, "Kristoffer", "Sveberg", "91918262", "kristoffersveberg@gmail.com", links),
            new(UserId.NewId, "lara", "Sveberg", "92628191", "larasveberg@gmail.com", links)
        };
        int totalCount = userDTOs.Count;

        _userServiceMock.Setup(x => x.GetUsersAsync(It.IsAny<UserQuery>())).ReturnsAsync(new UsersWithCountDTO(totalCount, userDTOs));

        // Act
        var result = await UserEndpointsLogic.GetUsersLogicAsync(
            query,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert    
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UsersWithCountDTO>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(totalCount, okResult.Value.TotalCount);
        Assert.Equal(userDTOs, okResult.Value.UserDTOs);
        okResult.Value.UserDTOs.Should().BeEquivalentTo(userDTOs, options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task GetUsersLogicAsync_WhenNoUsersExist_ReturnsNotFoundWithMessage()
    {
        // Arrange
        var query = new UserQuery(null, null, null, null);
        var usersAndCountDTO = new UsersWithCountDTO(0, []);

        _userServiceMock.Setup(x => x.GetUsersAsync(query)).ReturnsAsync(usersAndCountDTO);

        // Act
        var result = await UserEndpointsLogic.GetUsersLogicAsync(
            query,
            _userServiceMock.Object,
            _loggerMock.Object);

        //Assert
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<ErrorResponse>>(result);
        Assert.NotNull(notFoundResult.Value);
        Assert.Equal("No users found", notFoundResult.Value.Message);
    }

    #endregion GetUsers

    #region GetUserById

    [Fact]
    public async Task GetUserByIdLogicAsync_AsAdmin_WhenUserExists_ReturnsOkAndUserDTO()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);
        var links = new List<Link>();
        var userDTO = new UserDTO(userId, "Ketil", "Sveberg", "91914455", "ketilsveberg@gmail.com", links);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "Admin")
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, id, "UserIdAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Success());

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult);
        Assert.Equal(userDTO, okResult.Value);
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_AsAuthorizedUser_WhenUserExists_ReturnsOkAndUserDTO()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);
        var links = new List<Link>();
        var userDTO = new UserDTO(userId, "Ketil", "Sveberg", "91914455", "ketilsveberg@gmail.com", links);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, id, "UserIdAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Success());

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult);
        Assert.Equal(userDTO, okResult.Value);
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_WhenAuthorizationFails_ReturnsForbidden()
    {
        // Arrange        
        var id = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, id, "UserIdAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Failed);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_AsAdmin_WhenUserDoesNotExist_ReturnsNotFoundAndErrorResponse()
    {
        // Arrange
        var id = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "Admin")

        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, id, "UserIdAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Success());

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id))
            .ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var NotFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<ErrorResponse>>(result);
        Assert.NotNull(NotFoundResult.Value);
        Assert.Equal("User was not found", NotFoundResult.Value.Message);
    }

    #endregion GetUserById

    #region UpdateUserLogicAsync

    [Fact]
    public async Task UpdateUserLogicAsync_AsAdmin_WhenUpdateIsSuccessful_ReturnsOkAndUpdatedUserDTO()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);
        var links = new List<Link>();
        var userUpdateDTO = new UserUpdateDTO("Lars", "Larsen", "22223333", "lars@gmail.com");
        var userDTO = new UserDTO(userId, "Lars", "Larsen", "22223333", "lars@gmail.com", links);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "Admin")
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, id, "UserIdAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Success);

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO))
            .ReturnsAsync(userDTO);

        //Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            userUpdateDTO,
            id,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        //Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(userDTO, okResult.Value);
        Assert.Equal(userUpdateDTO.FirstName, okResult.Value.FirstName);
        Assert.Equal(userUpdateDTO.LastName, okResult.Value.LastName);
        Assert.Equal(userUpdateDTO.PhoneNumber, okResult.Value.PhoneNumber);
        Assert.Equal(userUpdateDTO.Email, okResult.Value.Email);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_AsAuthorizedUser_WhenUpdateIsSuccessful_ReturnsOkAndUpdatedUserDTO()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var links = new List<Link>();
        var userUpdateDTO = new UserUpdateDTO("Sarah", "Connor", "12344321", "sarah@example.com");
        var userDTO = new UserDTO(userId, "Sarah", "Connor", "12344321", "sarah@example.com", links);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, id, "UserIdAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Success());

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            userUpdateDTO,
            id,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(userDTO, okResult.Value);
        Assert.Equal(userUpdateDTO.FirstName, okResult.Value.FirstName);
        Assert.Equal(userUpdateDTO.LastName, okResult.Value.LastName);
        Assert.Equal(userUpdateDTO.PhoneNumber, okResult.Value.PhoneNumber);
        Assert.Equal(userUpdateDTO.Email, okResult.Value.Email);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_WhenAuthorizationFails_ReturnsForbidden()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var links = new List<Link>();
        var userUpdateDTO = new UserUpdateDTO("Sarah", "Connor", "12344321", "sarah@example.com");
        var userDTO = new UserDTO(userId, "Sarah", "Connor", "12344321", "sarah@example.com", links);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, id, "UserIdAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Failed());

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            userUpdateDTO,
            id,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_WhenUserCouldNotBeUpdated_ReturnsConflictAndErrorResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var links = new List<Link>();
        var userUpdateDTO = new UserUpdateDTO("Sarah", "Connor", "12344321", "sarahexample.com");

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, id, "UserIdAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Success);

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO))
            .ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            userUpdateDTO,
            id,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var conflictResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Conflict<ErrorResponse>>(result);
        Assert.NotNull(conflictResult.Value);
        Assert.Equal("User could not be updated", conflictResult.Value.Message);
    }

    #endregion UpdateUserLogicAsync

    #region DeleteUserLogicAsync

    [Fact]
    public async Task DeleteUserAsync_WhenSuccess_ReturnsOkAndDeletedUserDTO()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);
        var links = new List<Link>();
        var userDTO = new UserDTO(userId, "Bill", "Jones", "81625342", "billjones@test.no", links);

        _userServiceMock.Setup(x => x.DeleteUserAsync(id)).ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.DeleteUserLogicAsync(
            id,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(userDTO, okResult.Value);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenNotSuccessful_ReturnsConflictAndErrorResponse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);

        _userServiceMock.Setup(x => x.DeleteUserAsync(id)).ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.DeleteUserLogicAsync(
            id,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert
        var conflictResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Conflict<ErrorResponse>>(result);
        Assert.NotNull(conflictResult.Value);
        Assert.Equal("User could not be deleted", conflictResult.Value.Message);
    }

    #endregion DeleteUserLogicAsync

    #region RegisterUserLogicAsync

    [Fact]
    public async Task RegisterUserLogicAsync_WhenIsSuccess_ReturnsOkAndUserDTO()
    {
        // Arrange
        var userRegistrationDTO = new UserRegistrationDTO("Kristoffer", "Sveberg", "99999999", "kris@gmail.com", "secretPassword123!");
        var userId = UserId.NewId;
        var links = new List<Link>();
        var userDTO = new UserDTO(userId, "Kristoffer", "Sveberg", "99999999", "kris@gmail.com", links);

        _userServiceMock.Setup(x => x.RegisterUserAsync(userRegistrationDTO))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.RegisterUserLogicAsync(
            userRegistrationDTO,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(userDTO.FirstName, userRegistrationDTO.FirstName);
        Assert.Equal(userDTO.LastName, userRegistrationDTO.LastName);
        Assert.Equal(userDTO.PhoneNumber, userRegistrationDTO.PhoneNumber);
        Assert.Equal(userDTO.Email, userRegistrationDTO.Email);
    }

    [Fact]
    public async Task RegisterUserLogicAsync_WhenUserAlreadyExists_ReturnsConflictAndErrorResponse()
    {
        // Arrange
        var userRegistrationDTO = new UserRegistrationDTO("Kristoffer", "Sveberg", "(99999999", "kris@gmail.com", "secretPassword123!");

        _userServiceMock.Setup(x => x.RegisterUserAsync(userRegistrationDTO))
            .ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.RegisterUserLogicAsync(
            userRegistrationDTO,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert
        var conflictResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Conflict<ErrorResponse>>(result);
        Assert.NotNull(conflictResult.Value);
        Assert.Equal("User could not be registered", conflictResult.Value.Message);
    }

    #endregion RegisterUserLogicAsync

    #region UserLoginLogicAsync

    [Fact]
    public async Task UserLoginLogicAsync_WhenLoginSuccess_ReturnsOkAndValidToken()
    {
        // Arrange
        var loginDTO = new LoginDTO ( Email: "testuser@unittest.com", Password: "secretPassword123!" );
        var user = new User
        {
            Id = UserId.NewId,
            FirstName = "Testuser",
            LastName = "TestuserLastName",
            PhoneNumber = "71625353",
            Email = "testuser@unittest.com",
            HashedPassword = "someHashedPassword",
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        string token = "ValidTokenString";

        _userServiceMock.Setup(x => x.UserLoginAsync(loginDTO))
            .ReturnsAsync(token);

       // Act
        var result = await UserEndpointsLogic.UserLoginLogicAsync(
            loginDTO,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<TokenResponse>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(token, okResult.Value.Token);
    }

    [Fact]
    public async Task UserLoginLogicAsync_WhenLoginFails_ReturnsUnauthorized()
    {
        // Arrange
        var loginDTO = new LoginDTO ( Email: "testuser@unittest.com", Password: "secretPassword123!" );
        var user = new User
        {
            Id = UserId.NewId,
            FirstName = "Testuser",
            LastName = "TestuserLastName",
            PhoneNumber = "71625353",
            Email = "testuser@gmail.com",
            HashedPassword = "someHashedPassword",
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        _userServiceMock.Setup(x => x.UserLoginAsync(loginDTO))
            .ReturnsAsync((string?)null);

        // Act
        var result = await UserEndpointsLogic.UserLoginLogicAsync(
            loginDTO,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert
        var unauthorizedResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult>(result);
    }

    #endregion UserLoginLogicAsync

    #region UpdatePasswordLogicAsync

    [Fact]
    public async Task UpdatePasswordLogicAsync_AsAuthorizedUser_ReturnsOKAndValidToken()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "testuser@email.no", Password = "CurrentPass123!", NewPassword = "NewPass123!" };

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role,  "User"),
            new Claim(ClaimTypes.Name, "testuser@email.no")
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        string token = "ValidTokenString";

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, updatePasswordDTO, "UserNameAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Success());

        _userServiceMock.Setup(x => x.UpdatePasswordAsync(updatePasswordDTO))
            .ReturnsAsync(token);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<TokenResponse>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal("ValidTokenString", okResult.Value.Token);
    }

    [Fact]
    public async Task UpdatePasswordLogicAsync_WhenAuthorizationFails_ReturnsForbidden()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "epost@epost.no", Password = "GammeltPassord123!", NewPassword = "NyttPasssord123!" };
        var verifiedUser = new User { Id = UserId.NewId };

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "epost@epost.no"),
            new Claim(ClaimTypes.Role, "User")
        ]);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, updatePasswordDTO, "UserNameAccessPolicy")).ReturnsAsync(AuthorizationResult.Failed);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var forbidResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task UpdatePasswordLogicAsync_WhenPasswordCouldNotBeUpdated_ReturnsUnauthorized() 
    {
        //Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "epost@epost.no", Password = "GammeltPassord123!", NewPassword = "NyttPasssord123!" };

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "epost@epost.no"),
            new Claim(ClaimTypes.Role, "User")
        ]);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _authorizationServiceMock.Setup(x => x.AuthorizeAsync(claimsPrincipal, updatePasswordDTO, "UserNameAccessPolicy"))
            .ReturnsAsync(AuthorizationResult.Success);

        _userServiceMock.Setup(x => x.UpdatePasswordAsync(updatePasswordDTO)).ReturnsAsync((string?)null);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO, 
            _userServiceMock.Object,
            _authorizationServiceMock.Object,
            claimsPrincipal, 
            _loggerMock.Object);

        //Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult>(result);
    }

    #endregion UpdatePasswordLogicAsync
}
