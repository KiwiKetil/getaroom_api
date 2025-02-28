using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
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
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock = new();
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

        _userServiceMock.Setup(x => x.GetUsersAsync(It.IsAny<UserQuery>())).ReturnsAsync(new UsersAndCountDTO(totalCount, userDTOs));

        // Act
        var result = await UserEndpointsLogic.GetUsersLogicAsync(
            query,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert    
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UsersAndCountDTO>>(result);
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
        var usersAndCountDTO = new UsersAndCountDTO(0, []);

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
    public async Task GetUserByIdLogicAsync_AsAdmin_WhenUserExists_ReturnsOkAndValidData()
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

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id)).ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult);
        Assert.Equal(userDTO, okResult.Value);
        okResult.Value.Should().BeEquivalentTo(userDTO);  // userDTO is type Record, therefore prob not needed since Equals() compare by value anyways.
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_AsValidUser_WhenUserExists_ReturnsOkAndValidData()
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

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id)).ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult);
        Assert.Equal(userDTO, okResult.Value);
        okResult.Value.Should().BeEquivalentTo(userDTO);  // userDTO is type Record, therefore prob not needed since Equals() compare by value anyways.
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_AsAdmin_WhenUserDoesNotExist_ReturnsNotFoundAndMessage()
    {
        // Arrange
        var id = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "Admin")

        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id)).ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var NotFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<ErrorResponse>>(result);
        Assert.NotNull(NotFoundResult.Value);
        Assert.Equal("User was not found", NotFoundResult.Value.Message);
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_WhenUserIdDoesNotMatchTargetId_ReturnsForbidden()
    {
        // Arrange        
        var id = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id)).ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_WhenNameIdentifierClaimIsNull_ReturnsForbidden()
    {
        // Arrange        
        var id = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id)).ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_WhenUserRoleIsMissingFromClaims_ReturnsForbidden()
    {
        // Arrange        
        var id = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "SomeRole")
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _userServiceMock.Setup(x => x.GetUserByIdAsync(id)).ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(
            id,
            _userServiceMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    #endregion GetUserById

    #region UpdateUserLogicAsync

    [Fact]
    public async Task UpdateUserLogicAsync_AsAdmin_WhenUpdateIsSuccessful_ReturnsOkAndValidData()
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

        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();
        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
             .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO))
            .ReturnsAsync(userDTO);

        //Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            id,
            userUpdateDTO,
            _userServiceMock.Object,
            validatorMock.Object,
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
    public async Task UpdateUserLogicAsync_AsValidUser_WhenUpdateIsSuccessful_ReturnsOkAndValidData()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

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

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            id,
            userUpdateDTO,
            _userServiceMock.Object,
            validatorMock.Object,
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
    public async Task UpdateUserLogicAsync_WhenValidationFails_ReturnsBadRequestAndErrors()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

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
        var userDTO = new UserDTO(userId, "Sarah", "Connor", "12344321", "sarah@example.com", links);

        var errors = new List<ValidationFailure>(
        [
            new ValidationFailure("Email", "Email is Invalid")
        ]);
        var expectedErrorMessages = new List<string> { "Email is Invalid" };

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            id,
            userUpdateDTO,
            _userServiceMock.Object,
            validatorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<ErrorResponse>>(result);
        Assert.NotNull(badRequestResult.Value);
        Assert.Equal(expectedErrorMessages, badRequestResult.Value.Errors);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_WhenUserNameIdentifierClaimDoesNotMatchTarget_ReturnsForbidden()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

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

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            id,
            userUpdateDTO,
            _userServiceMock.Object,
            validatorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_WhenNameIdentifierClaimIsNull_ReturnsForbidden()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

        var id = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var userUpdateDTO = new UserUpdateDTO("Sarah", "Connor", "12344321", "sarah@example.com");

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            id,
            userUpdateDTO,
            _userServiceMock.Object,
            validatorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_WhenUserRoleIsMissingFromClaims_ReturnsForbidden()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

        var id = Guid.Parse("d77ac10b-58cc-4372-a567-0e02b2c3d488");

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, Guid.Parse("d77ac10b-58cc-4372-a567-0e02b2c3d488").ToString()),
            new Claim(ClaimTypes.Role, "SomeRole")
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var userUpdateDTO = new UserUpdateDTO("Sarah", "Connor", "12344321", "sarah@example.com");

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            id,
            userUpdateDTO,
            _userServiceMock.Object,
            validatorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_WhenResultIsNull_ReturnsConflictAndMessage()
    {
        // Arrange
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

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

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(x => x.UpdateUserAsync(id, userUpdateDTO))
            .ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(
            id,
            userUpdateDTO,
            _userServiceMock.Object,
            validatorMock.Object,
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
    public async Task DeleteUserAsync_WhenSuccess_ReturnsOkAndValidData()
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
    public async Task DeleteUserAsync_WhenNotSuccessful_ReturnsConflictAndMessage()
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
    public async Task RegisterUserLogicAsync_WhenIsSuccess_ReturnsOkAndValidData()
    {
        // Arrange
        var userRegistrationDTO = new UserRegistrationDTO("Kristoffer", "Sveberg", "99999999", "kris@gmail.com", "secretPassword123!");
        var userId = UserId.NewId;
        var links = new List<Link>();
        var userDTO = new UserDTO(userId, "Kristoffer", "Sveberg", "99999999", "kris@gmail.com", links);

        var validatorMock = new Mock<IValidator<UserRegistrationDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(userRegistrationDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(x => x.RegisterUserAsync(userRegistrationDTO))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.RegisterUserLogicAsync(
            userRegistrationDTO,
            validatorMock.Object,
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
    public async Task RegisterUserLogicAsync_WhenValidationFails_ReturnsBadRequestAndErrors()
    {
        // Arrange
        var userRegistrationDTO = new UserRegistrationDTO("Kristoffer", "Sveberg", "99999999", "kris@gmail.com", "secretPassword123!");
        var userId = UserId.NewId;
        var links = new List<Link>();
        var userDTO = new UserDTO(userId, "Kristoffer", "Sveberg", "99999999", "kris@gmail.com", links);
        var errors = new List<ValidationFailure>(
        [
            new ValidationFailure("Password", "Password must be 8-24 characters, include at least 1 number, 1 uppercase," +
                                                  " 1 lowercase, and 1 special character ('! ? * # _ -')")
        ]);
        var expectedErrors = new List<string> {"Password must be 8-24 characters, include at least 1 number, 1 uppercase," +
                                                  " 1 lowercase, and 1 special character ('! ? * # _ -')" };

        var validatorMock = new Mock<IValidator<UserRegistrationDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(userRegistrationDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(errors));

        _userServiceMock.Setup(x => x.RegisterUserAsync(userRegistrationDTO))
            .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.RegisterUserLogicAsync(
            userRegistrationDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<ErrorResponse>>(result);
        Assert.NotNull(badRequestResult.Value);
        Assert.Equal(expectedErrors, badRequestResult.Value.Errors);
    }

    [Fact]
    public async Task RegisterUserLogicAsync_WhenUserAlreadyExists_ReturnsConflictAndMessage()
    {
        // Arrange
        var userRegistrationDTO = new UserRegistrationDTO("Kristoffer", "Sveberg", "(99999999", "kris@gmail.com", "secretPassword123!");

        var validatorMock = new Mock<IValidator<UserRegistrationDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(userRegistrationDTO, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _userServiceMock.Setup(x => x.RegisterUserAsync(userRegistrationDTO)).ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.RegisterUserLogicAsync(
            userRegistrationDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _loggerMock.Object);

        // Assert
        var conflictResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Conflict<ErrorResponse>>(result);
        Assert.NotNull(conflictResult.Value);
        Assert.Equal("User already exists", conflictResult.Value.Message);
    }

    #endregion RegisterUserLogicAsync

    #region UserLoginLogicAsync

    //[Fact]
    //public async Task UserLoginLogicAsync_WhenLoginSuccess_AndUserHasUpdatedPassword_ReturnsOkWithValidToken()
    //{
    //    // Arrange
    //    var loginDTO = new LoginDTO { Email = "testuser@unittest.com", Password = "secretPassword123!" };
    //    var user = new User
    //    {
    //        Id = UserId.NewId,
    //        FirstName = "Testuser",
    //        LastName = "TestuserLastName",
    //        PhoneNumber = "71625353",
    //        Email = "testuser@unittest.com",
    //        HashedPassword = "someHashedPassword",
    //        Created = DateTime.UtcNow,
    //        Updated = DateTime.UtcNow
    //    };

    //    List<UserRole> userRoles = [];

    //    var validatorMock = new Mock<IValidator<LoginDTO>>();
    //    validatorMock.Setup(x => x.ValidateAsync(loginDTO, It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(new ValidationResult());

    //    var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
    //    passwordVerificationServiceMock.Setup(x => x.VerifyPassword(loginDTO, user)).Returns(true);

    //    var token = new TokenResponse(Token: "tokenStringWithClaimPasswordUpdatedTrue");
    //    var tokenGeneratorMock = new Mock<ITokenGenerator>();
    //    tokenGeneratorMock.Setup(x => x.GenerateToken(user, true, userRoles)).Returns(token.Token);

    //    _userServiceMock.Setup(x => x.HasUpdatedPassword(user.Id)).ReturnsAsync(true);
    //    _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.Email)).ReturnsAsync(user);
    //    _userRoleRepositoryMock.Setup(x => x.GetUserRoles(user.Id)).ReturnsAsync(userRoles);

    //    // Act
    //    var result = await UserEndpointsLogic.UserLoginLogicAsync(
    //        loginDTO,
    //        validatorMock.Object,
    //        _userServiceMock.Object,
    //        _userRepositoryMock.Object,
    //        _userRoleRepositoryMock.Object,
    //        passwordVerificationServiceMock.Object,
    //        tokenGeneratorMock.Object,
    //        _loggerMock.Object);

    //    // Assert
    //    var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<TokenResponse>>(result);
    //    Assert.NotNull(okResult.Value);
    //    Assert.Equal(token.Token, okResult.Value.Token);
    //}

    //[Fact]
    //public async Task UserLoginLogicAsync_WhenLoginSuccess_AndUserHasNotUpdatedPassword_ReturnsOkWithValidToken()
    //{
    //    // Arrange
    //    var loginDTO = new LoginDTO { Email = "testuser@unittest.com", Password = "secretPassword123!" };
    //    var user = new User
    //    {
    //        Id = UserId.NewId,
    //        FirstName = "Testuser",
    //        LastName = "TestuserLastName",
    //        PhoneNumber = "71625353",
    //        Email = "testuser@gmail.com",
    //        HashedPassword = "someHashedPassword",
    //        Created = DateTime.UtcNow,
    //        Updated = DateTime.UtcNow
    //    };

    //    List<UserRole> userRoles = [];

    //    var validatorMock = new Mock<IValidator<LoginDTO>>();
    //    validatorMock.Setup(x => x.ValidateAsync(loginDTO, It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(new ValidationResult());

    //    var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
    //    passwordVerificationServiceMock.Setup(x => x.VerifyPassword(loginDTO, user)).Returns(true);

    //    var token = new TokenResponse(Token: "tokenStringWithClaimPasswordUpdatedFalse");
    //    var tokenGeneratorMock = new Mock<ITokenGenerator>();
    //    tokenGeneratorMock.Setup(x => x.GenerateToken(user, false, userRoles)).Returns(token.Token);

    //    _userServiceMock.Setup(x => x.HasUpdatedPassword(user.Id)).ReturnsAsync(false);
    //    _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.Email)).ReturnsAsync(user);
    //    _userRoleRepositoryMock.Setup(x => x.GetUserRoles(user.Id)).ReturnsAsync(userRoles);

    //    // Act
    //    var result = await UserEndpointsLogic.UserLoginLogicAsync(
    //        loginDTO,
    //        validatorMock.Object,
    //        _userServiceMock.Object,
    //        _userRepositoryMock.Object,
    //        _userRoleRepositoryMock.Object,
    //        passwordVerificationServiceMock.Object,
    //        tokenGeneratorMock.Object,
    //        _loggerMock.Object
    //        );

    //    // Assert
    //    var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<TokenResponse>>(result);
    //    Assert.NotNull(okResult.Value);
    //    Assert.Equal(token.Token, okResult.Value.Token);
    //}

    //[Fact]
    //public async Task UserLoginLogicAsync_WhenValidationFails_ReturnsBadRequestAndErrors()
    //{
    //    // Arrange
    //    var validatorMock = new Mock<IValidator<LoginDTO>>();
    //    var errors = new ValidationResult(
    //    [
    //        new ValidationFailure("Error", "testError")
    //    ]);
    //    validatorMock.Setup(x => x.ValidateAsync(It.IsAny<LoginDTO>(), It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(errors);

    //    var actualErrors = errors.Errors.Select(e => e.ErrorMessage).ToList();

    //    var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
    //    var tokenGeneratorMock = new Mock<ITokenGenerator>();

    //    // Act
    //    var result = await UserEndpointsLogic.UserLoginLogicAsync(
    //        It.IsAny<LoginDTO>(),
    //        validatorMock.Object,
    //        _userServiceMock.Object,
    //        _userRepositoryMock.Object,
    //        _userRoleRepositoryMock.Object,
    //        passwordVerificationServiceMock.Object,
    //        tokenGeneratorMock.Object,
    //        _loggerMock.Object);

    //    // Assert
    //    var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<ErrorResponse>>(result);
    //    Assert.NotNull(badRequestResult.Value);
    //    Assert.Equal(actualErrors, badRequestResult.Value.Errors);
    //    tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    //}

    //[Fact]
    //public async Task UserLoginLogicAsync_WhenuserIsNotFound_ReturnsNotFoundAndMessage()
    //{
    //    // Arrange
    //    var loginDTO = new LoginDTO { Email = "random@email.com", Password = "randomPassword123!" };
    //    var validatorMock = new Mock<IValidator<LoginDTO>>();
    //    var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
    //    var tokenGeneratorMock = new Mock<ITokenGenerator>();
    //    var expectedString = "User not found";

    //    _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.Email)).ReturnsAsync((User?)null);
    //    validatorMock.Setup(x => x.ValidateAsync(loginDTO, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

    //    // Act
    //    var result = await UserEndpointsLogic.UserLoginLogicAsync(
    //        loginDTO,
    //        validatorMock.Object,
    //        _userServiceMock.Object,
    //        _userRepositoryMock.Object,
    //        _userRoleRepositoryMock.Object,
    //        passwordVerificationServiceMock.Object,
    //        tokenGeneratorMock.Object,
    //        _loggerMock.Object);

    //    // Assert
    //    var notFoundResponse = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<ErrorResponse>>(result);
    //    Assert.NotNull(notFoundResponse.Value);
    //    Assert.Equal(expectedString, notFoundResponse.Value.Message);
    //    tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    //}

    //[Fact]
    //public async Task UserLoginLogicAsync_WhenPasswordVerificationFails_ReturnsUnauthorized()
    //{
    //    // Arrange
    //    var loginDTO = new LoginDTO { Email = "testuser@unittest.com", Password = "secretPassword123!" };
    //    var user = new User
    //    {
    //        Id = UserId.NewId,
    //        FirstName = "Testuser",
    //        LastName = "TestuserLastName",
    //        PhoneNumber = "71625353",
    //        Email = "testuser@gmail.com",
    //        HashedPassword = "someHashedPassword",
    //        Created = DateTime.UtcNow,
    //        Updated = DateTime.UtcNow
    //    };

    //    var validatorMock = new Mock<IValidator<LoginDTO>>();
    //    validatorMock.Setup(x => x.ValidateAsync(loginDTO, It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(new ValidationResult());

    //    var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
    //    passwordVerificationServiceMock.Setup(x => x.VerifyPassword(loginDTO, user)).Returns(false);

    //    var tokenGeneratorMock = new Mock<ITokenGenerator>();

    //    _userServiceMock.Setup(x => x.HasUpdatedPassword(user.Id)).ReturnsAsync(false);
    //    _userRepositoryMock.Setup(x => x.GetUserByEmailAsync(loginDTO.Email)).ReturnsAsync(user);

    //    // Act
    //    var result = await UserEndpointsLogic.UserLoginLogicAsync(
    //        loginDTO,
    //        validatorMock.Object,
    //        _userServiceMock.Object,
    //        _userRepositoryMock.Object,
    //        _userRoleRepositoryMock.Object,
    //        passwordVerificationServiceMock.Object,
    //        tokenGeneratorMock.Object,
    //        _loggerMock.Object);

    //    // Assert
    //    var unauthorizedResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult>(result);
    //    tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    //}

    #endregion UserLoginLogicAsync

    #region UpdatePasswordLogicAsync

    [Fact]
    public async Task UpdatePasswordLogicAsync_AsValidUser_ReturnsOKAndValidToken()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "testuser@email.no", Password = "CurrentPass123!", NewPassword = "NewPass123!" };

        var validatorMock = new Mock<IValidator<UpdatePasswordDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(updatePasswordDTO, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role,  "User"),
            new Claim(ClaimTypes.Name, "testuser@email.no")
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
        passwordVerificationServiceMock.Setup(x => x.VerifyPassword(updatePasswordDTO, It.IsAny<User>())).Returns(true); ;

        var tokengeneratorMock = new Mock<ITokenGenerator>();
        tokengeneratorMock.Setup(x => x.GenerateToken(It.IsAny<User>(), true, It.IsAny<IEnumerable<UserRole>>())).Returns("tokenStringValue");

        _userServiceMock.Setup(x => x.UpdatePasswordAsync(updatePasswordDTO, It.IsAny<User>())).ReturnsAsync(true);
        _userServiceMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Email)).ReturnsAsync(new User { Email = updatePasswordDTO.Email });

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _userRoleRepositoryMock.Object,
            passwordVerificationServiceMock.Object,
            tokengeneratorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<TokenResponse>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal("tokenStringValue", okResult.Value.Token);
    }

    [Fact]
    public async Task UpdatePasswordLogicAsync_WhenValidationFails_ReturnsBadRequest()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "epost@epost.no", Password = "GammeltPassord123!", NewPassword = "NyttPasssord123!" };

        var validatorMock = new Mock<IValidator<UpdatePasswordDTO>>();
        var errors = new ValidationResult(
        [
            new ValidationFailure(nameof(UpdatePasswordDTO.Email), "An Error")
        ]);
        var expectedErrors = errors.Errors.Select(x => x.ErrorMessage).ToList();
        validatorMock.Setup(x => x.ValidateAsync(updatePasswordDTO, It.IsAny<CancellationToken>())).ReturnsAsync((errors));

        var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();

        var tokenGeneratorMock = new Mock<ITokenGenerator>();
        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "epost@epost.no"),
            new Claim(ClaimTypes.Role, "User")
        ]);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _userRoleRepositoryMock.Object,
            passwordVerificationServiceMock.Object,
            tokenGeneratorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<ErrorResponse>>(result);
        Assert.NotNull(badRequestResult.Value);
        Assert.Equal(expectedErrors, badRequestResult.Value.Errors);
        tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePasswordLogicAsync_WhenUserNameClaimIsNull_ReturnsForbidden()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "email@email.com", Password = "currentPassword", NewPassword = "NewPassword123!" };

        var validatorMock = new Mock<IValidator<UpdatePasswordDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(updatePasswordDTO, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
        var tokenGeneratorMock = new Mock<ITokenGenerator>();
        var claimsPrincipal = new ClaimsPrincipal();


        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _userRoleRepositoryMock.Object,
            passwordVerificationServiceMock.Object,
            tokenGeneratorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        //Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
        tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);

    }

    [Fact]
    public async Task UpdatePasswordLogicAsync_WhenNameClaimDoesNotMatchTarget_ReturnsForbidden()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "someemail@abc.no", Password = "currPass", NewPassword = "Newpass123!" };

        var validatorMock = new Mock<IValidator<UpdatePasswordDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(updatePasswordDTO, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
        var tokenGeneratorMock = new Mock<ITokenGenerator>();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role,  "User"),
            new Claim(ClaimTypes.Name, "notmatching@email.no")
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _userRoleRepositoryMock.Object,
            passwordVerificationServiceMock.Object,
            tokenGeneratorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        //Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
        tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePasswordLogicAsync_WhenUserRoleIsMissingFromClaims_ReturnsForbidden()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "epost@epost.no", Password = "GammeltPassord123!", NewPassword = "NyttPasssord123!" };

        var validatorMock = new Mock<IValidator<UpdatePasswordDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(updatePasswordDTO, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
        var tokenGeneratorMock = new Mock<ITokenGenerator>();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "epost@epost.no"),
            new Claim(ClaimTypes.Role, "SomeRole")
        ]);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _userRoleRepositoryMock.Object,
            passwordVerificationServiceMock.Object,
            tokenGeneratorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
        tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePasswordLogic_WhenUserIsNotFound_ReturnsNotFoundAndMessage()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "epost@epost.no", Password = "GammeltPassord123!", NewPassword = "NyttPasssord123!" };

        var validatorMock = new Mock<IValidator<UpdatePasswordDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(updatePasswordDTO, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
        var tokenGeneratorMock = new Mock<ITokenGenerator>();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "epost@epost.no"),
            new Claim(ClaimTypes.Role, "User")
        ]);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var expectedMessage = "User not found";

        _userServiceMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Email)).ReturnsAsync((User?)null);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _userRoleRepositoryMock.Object,
            passwordVerificationServiceMock.Object,
            tokenGeneratorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<ErrorResponse>>(result);
        Assert.NotNull(notFoundResult.Value);
        Assert.Equal(expectedMessage, notFoundResult.Value.Message);
        tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePasswordLogicAsync_WhenPasswordVerificationFails_ReturnsUnauthorized()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "epost@epost.no", Password = "GammeltPassord123!", NewPassword = "NyttPasssord123!" };
        var verifiedUser = new User { Id = UserId.NewId };

        var validatorMock = new Mock<IValidator<UpdatePasswordDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(updatePasswordDTO, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
        passwordVerificationServiceMock.Setup(x => x.VerifyPassword(updatePasswordDTO, It.IsAny<User>())).Returns(false);

        var tokenGeneratorMock = new Mock<ITokenGenerator>();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "epost@epost.no"),
            new Claim(ClaimTypes.Role, "User")
        ]);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _userServiceMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Email)).ReturnsAsync(verifiedUser);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _userRoleRepositoryMock.Object,
            passwordVerificationServiceMock.Object,
            tokenGeneratorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var unauthorizedResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult>(result);
        tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePasswordLogicAsync_WhenUpdatingPasswordFails_ReturnsBadRequestAndMessage()
    {
        // Arrange
        var updatePasswordDTO = new UpdatePasswordDTO { Email = "epost@epost.no", Password = "GammeltPassord123!", NewPassword = "NyttPasssord123!" };
        var verifiedUser = new User { Id = UserId.NewId };

        var validatorMock = new Mock<IValidator<UpdatePasswordDTO>>();
        validatorMock.Setup(x => x.ValidateAsync(updatePasswordDTO, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var passwordVerificationServiceMock = new Mock<IPasswordVerificationService>();
        passwordVerificationServiceMock.Setup(x => x.VerifyPassword(updatePasswordDTO, It.IsAny<User>())).Returns(true);

        var tokenGeneratorMock = new Mock<ITokenGenerator>();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, "epost@epost.no"),
            new Claim(ClaimTypes.Role, "User")
        ]);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _userServiceMock.Setup(x => x.GetUserByEmailAsync(updatePasswordDTO.Email)).ReturnsAsync(verifiedUser);
        _userServiceMock.Setup(x => x.UpdatePasswordAsync(updatePasswordDTO, It.IsAny<User>())).ReturnsAsync(false);

        // Act
        var result = await UserEndpointsLogic.UpdatePasswordLogicAsync(
            updatePasswordDTO,
            validatorMock.Object,
            _userServiceMock.Object,
            _userRoleRepositoryMock.Object,
            passwordVerificationServiceMock.Object,
            tokenGeneratorMock.Object,
            claimsPrincipal,
            _loggerMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<ErrorResponse>>(result);
        Assert.NotNull(badRequestResult.Value);
        Assert.Equal("Password could not be updated. Please check your username or password and try again.", badRequestResult.Value.Message);
        tokenGeneratorMock.Verify(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<IEnumerable<UserRole>>()), Times.Never);
    }

    #endregion UpdatePasswordLogicAsync

}
