using Microsoft.Extensions.Logging;
using Moq;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Services;

namespace RoomSchedulerAPI.UnitTests.ServicesTests;
public class UserServiceTests
{
    private readonly Mock<ILogger<UserService>> _loggerMock = new();


    [Fact]
    public async Task GetUsersAsync_ReturnsUsersWithCountDTO() 
    {
        // Arrange
        var userQuery = new UserQuery( "Ketil", null, null, null );


        //Act



        //Assert
    
    }
}
