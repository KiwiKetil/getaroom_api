using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MySqlX.XDevAPI.Common;
using RoomSchedulerAPI.Features.Endpoints.Logic;
using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.UnitTests.EndpointsTests;
public class UserEndpointsLogicTests
{
    #region GetUsers

    [Fact]
    public async Task GetUsersLogicAsync_ReturnsOk_WhenUsersExist()
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
    public async Task GetUsersLogicAsync_ReturnsNotFound_WhenNoUsersExist() 
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var loggerMock = new Mock<ILogger<Program>>();
        UserQuery query = new(null,null,null,null);
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
    public async Task GetUserByIdLogicAsync() 
    {
        // Arrange



        // Act


        // Assert
    }

    #endregion GetUserById
}
