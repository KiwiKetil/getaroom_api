using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Org.BouncyCastle.Tls;
using RoomSchedulerAPI.Features.Endpoints.Logic;
using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Services.Interfaces;
using System.Security.Claims;

namespace RoomSchedulerAPI.UnitTests.EndpointsTests;
public class UserEndpointsLogicTests
{
    #region GetUsers

    [Fact]
    public async Task GetUsersLogicAsync_WhenUsersExist_ReturnsOkWithValidData()
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
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

        userServiceMock.Setup(x => x.GetUsersAsync(It.IsAny<UserQuery>())).ReturnsAsync(new UsersAndCountDTO(totalCount, userDTOs));

        // Act
        var result = await UserEndpointsLogic.GetUsersLogicAsync(userServiceMock.Object, query, loggerMock.Object);

        // Assert    
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UsersAndCountDTO>>(result);
        Assert.NotNull(okResult.Value);
        Assert.Equal(totalCount, okResult.Value.TotalCount);
        Assert.Equal(userDTOs, okResult.Value.Data);
        okResult.Value.Data.Should().BeEquivalentTo(userDTOs, options => options.WithStrictOrdering());      
    }

    [Fact]
    public async Task GetUsersLogicAsync_WhenNoUsersExist_ReturnsNotFoundWithEmptyData()
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
        UserQuery query = new(null, null, null, null);
        var usersAndCountDTO = new UsersAndCountDTO(0, []);

        userServiceMock.Setup(x => x.GetUsersAsync(query)).ReturnsAsync(usersAndCountDTO);

        // Act
        var result = await UserEndpointsLogic.GetUsersLogicAsync(userServiceMock.Object, query, loggerMock.Object);

        //Assert
        var notFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
        Assert.NotNull(notFoundResult.Value);
        Assert.Equal("No users found", notFoundResult.Value);
    }

    #endregion GetUsers

    #region GetUserById

    [Fact]
    public async Task GetUserByIdLogicAsync_AsAdmin_WhenUserExists_ReturnsOkAndValidData()
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();

        var userGuid = Guid.NewGuid();
        var userId = new UserId(userGuid);
        var links = new List<Link>();
        var userDTO = new UserDTO(userId, "Ketil", "Sveberg", "91914455", "ketilsveberg@gmail.com", links);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "Admin")
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        userServiceMock.Setup(x => x.GetUserByIdAsync(userGuid)).ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(userGuid, userServiceMock.Object, claimsPrincipal, loggerMock.Object);

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
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();

        var userGuid = Guid.NewGuid();
        var userId = new UserId(userGuid);
        var links = new List<Link>();
        var userDTO = new UserDTO(userId, "Ketil", "Sveberg", "91914455", "ketilsveberg@gmail.com", links);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, userGuid.ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        userServiceMock.Setup(x => x.GetUserByIdAsync(userGuid)).ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(userGuid, userServiceMock.Object, claimsPrincipal, loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult);
        Assert.Equal(userDTO, okResult.Value);
        okResult.Value.Should().BeEquivalentTo(userDTO);  // userDTO is type Record, therefore prob not needed since Equals() compare by value anyways.
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_AsAdmin_WhenUserDoesNotExist_ReturnsNotFound() 
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();

        var guid = Guid.NewGuid();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "Admin")
            
        ],"TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        userServiceMock.Setup(x => x.GetUserByIdAsync(guid)).ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(guid, userServiceMock.Object, claimsPrincipal,loggerMock.Object);

        // Assert
        var NotFoundResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<string>>(result);
        Assert.NotNull(NotFoundResult);
        Assert.Equal("User was not found", NotFoundResult.Value);
    }

    [Fact]
    public async Task GetUserByIdLogicAsync_WhenUserIsNotAuthorized_ReturnsForbidden()
    {
        // Arrange|
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
        
        var userGuid = Guid.NewGuid();
        var userId = new UserId(userGuid);
        var links = new List<Link>();

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) // user Idclaim wont match target id => Forbidden()
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        userServiceMock.Setup(x => x.GetUserByIdAsync(userGuid)).ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.GetUserByIdLogicAsync(userGuid, userServiceMock.Object, claimsPrincipal, loggerMock.Object);

        // Assert
        var forbidResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    #endregion GetUserById

    #region UpdateUserLogicAsync

    [Fact]
    public async Task UpdateUserLogicAsync_AsAdmin_WhenUpdateIsSuccessful_ReturnsOkAndValidData() 
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);
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

        userServiceMock.Setup(x => x.UpdateUserAsync(guid, userUpdateDTO)).ReturnsAsync(userDTO);

        //Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(guid, userUpdateDTO, userServiceMock.Object, validatorMock.Object, claimsPrincipal, loggerMock.Object);

        //Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.NotNull(okResult);
        Assert.Equal(userDTO, okResult.Value);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_AsValidUser_WhenUpdateIsSuccessful_ReturnsOkAndValidData() 
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, guid.ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var links = new List<Link>();
        var userUpdateDTO = new UserUpdateDTO("Sarah", "Connor", "12344321", "sarah@example.com");
        var userDTO = new UserDTO(userId, "Sarah", "Connor", "12344321", "sarah@example.com", links);

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
         .ReturnsAsync(new ValidationResult());

        userServiceMock.Setup(x => x.UpdateUserAsync(guid, userUpdateDTO))
         .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(guid, userUpdateDTO, userServiceMock.Object, validatorMock.Object, claimsPrincipal, loggerMock.Object);

        // Assert
        var okResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<UserDTO>>(result);
        Assert.Equal(userDTO, okResult.Value);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_WhenUserIsNotAuthorized_ReturnsForbidden() 
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()) // user Idclaim wont match target id => Forbidden()
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var links = new List<Link>();
        var userUpdateDTO = new UserUpdateDTO("Sarah", "Connor", "12344321", "sarah@example.com");
        var userDTO = new UserDTO(userId, "Sarah", "Connor", "12344321", "sarah@example.com", links);

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
         .ReturnsAsync(new ValidationResult());

        userServiceMock.Setup(x => x.UpdateUserAsync(guid, userUpdateDTO))
         .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(guid, userUpdateDTO, userServiceMock.Object, validatorMock.Object, claimsPrincipal, loggerMock.Object);

        // Assert
        var forbidResultResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ForbidHttpResult>(result);
    }

    [Fact]
    public async Task UpdateUserLogicAsync_WhenValidationFails_ReturnsBadRequestAndErrors() 
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, guid.ToString()) 
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

        userServiceMock.Setup(x => x.UpdateUserAsync(guid, userUpdateDTO))
         .ReturnsAsync(userDTO);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(guid, userUpdateDTO, userServiceMock.Object, validatorMock.Object, claimsPrincipal, loggerMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.BadRequest<List<string>>>(result);
        Assert.Equal(expectedErrorMessages, badRequestResult.Value);
    }

    // test Results.Problem (null)
    [Fact]
    public async Task UpdateUserLogicAsync_WhenResultIsNull_ShouldReturnProblemWithDetails() 
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
        var validatorMock = new Mock<IValidator<UserUpdateDTO>>();

        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        var claimsIdentity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.NameIdentifier, guid.ToString())
        ], "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var links = new List<Link>();
        var userUpdateDTO = new UserUpdateDTO("Sarah", "Connor", "12344321", "sarahexample.com");

        validatorMock.Setup(v => v.ValidateAsync(userUpdateDTO, It.IsAny<CancellationToken>()))
         .ReturnsAsync(new ValidationResult());

        userServiceMock.Setup(x => x.UpdateUserAsync(guid, userUpdateDTO))
         .ReturnsAsync((UserDTO?)null);

        // Act
        var result = await UserEndpointsLogic.UpdateUserLogicAsync(guid, userUpdateDTO, userServiceMock.Object, validatorMock.Object, claimsPrincipal, loggerMock.Object);

        // Assert
        var problemresult = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(result);
        Assert.Equal("An issue occured", problemresult.ProblemDetails.Title);
        Assert.Equal(409, problemresult.ProblemDetails.Status);
        Assert.Equal("User could not be updated", problemresult.ProblemDetails.Detail);
    }

    #endregion UpdateUserLogicAsync
}
