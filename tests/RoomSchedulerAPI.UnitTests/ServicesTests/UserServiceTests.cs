using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using RoomSchedulerAPI.Features.AutoMapper;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using RoomSchedulerAPI.Features.Repositories.Interfaces;
using RoomSchedulerAPI.Features.Services;
using RoomSchedulerAPI.Features.Services.Interfaces;

namespace RoomSchedulerAPI.UnitTests.ServicesTests;
public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUserRoleRepository> _userRoleRepositoryMock = new();
    private readonly Mock<IPasswordVerificationService> _passwordVerificationServiceMock = new();
    private readonly Mock<IPasswordHistoryRepository> _passwordHistoryRepositoryMock = new();
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock = new();
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _loggerMock = new();

    public UserServiceTests()
    {

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>(); // Add your mapping profile(s)
        });
        IMapper mapper = mapperConfig.CreateMapper();

        _userService = new UserService(
            _userRepositoryMock.Object, 
            _userRoleRepositoryMock.Object,
            _passwordVerificationServiceMock.Object,
            _passwordHistoryRepositoryMock.Object,
            _tokenGeneratorMock.Object,
            _mapper = mapper,
            _loggerMock.Object
            );
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsUsersWithCountDTO() 
    {
        // Arrange
        var userQuery = new UserQuery( null, null, null, null );

        List<User> users = [
            new User { FirstName = "Jim" },
            new User { FirstName = "Michelle" }
            ];
        int totalCount = 10;

        _userRepositoryMock.Setup(x => x.GetUsersAsync(userQuery)).ReturnsAsync((users, totalCount));

        //Act
        var result = await _userService.GetUsersAsync(userQuery);

        //Assert
        var usersWithCountDTO = Assert.IsType<UsersWithCountDTO>(result);
        Assert.Equal(totalCount, usersWithCountDTO.TotalCount);        
        Assert.Equal(usersWithCountDTO.UserDTOs.First().FirstName, users[0].FirstName);        
    }
}
