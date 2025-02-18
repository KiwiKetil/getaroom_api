using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
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

        // alternativ til fluentassertions:
        //foreach (var expectedUser in userDTOs)
        //{
        //    var actualUser = okResult.Value.Data.FirstOrDefault(u => u.Email == expectedUser.Email);
        //    Assert.NotNull(actualUser);
        //    Assert.Equal(expectedUser.FirstName, actualUser.FirstName);
        //    Assert.Equal(expectedUser.LastName, actualUser.LastName);
        //    Assert.Equal(expectedUser.PhoneNumber, actualUser.PhoneNumber);
        //    Assert.Equal(expectedUser.Email, actualUser.Email);
        //    Assert.Equal(expectedUser.Links, actualUser.Links);
        //}
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
        okResult.Value.Should().BeEquivalentTo(userDTO);  // redundant bec userDTO is record and by design overrides the default equality, so it compares by value rather than ref.
    }

    public async Task GetUserByIdLogicAsync_AsAdmin_WhenUserDoesNotExist_ReturnsNotFound() 
    {
        // Arrange



        // Act


        // Assert


    }

    //test for "forbid" også - just one

    #endregion GetUserById
}
