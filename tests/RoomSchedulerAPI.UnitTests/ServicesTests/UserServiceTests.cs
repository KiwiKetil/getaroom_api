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
    private readonly Mock<ILogger<UserService>> _loggerMock = new();

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
            _loggerMock.Object
            );
    }

    #region GetUsersAsync

    [Fact]
    public async Task GetUsersAsync_ReturnsUsersWithCountDTO_WithValidData()
    {
        // Arrange
        var userQuery = new UserQuery(null, null, null, null);

        List<User> users = [
            new User { FirstName = "Jim", LastName = "Moore" },
            new User { FirstName = "Michelle", LastName = "Andersson" }
            ];
        int totalCount = users.Count;

        _userRepositoryMock.Setup(x => x.GetUsersAsync(userQuery))
            .ReturnsAsync((users, totalCount));

        //Act
        var result = await _userService.GetUsersAsync(userQuery);

        //Assert
        var usersWithCountDTO = Assert.IsType<UsersWithCountDTO>(result);
        Assert.Equal(totalCount, usersWithCountDTO.TotalCount);
        Assert.Equal(totalCount, usersWithCountDTO.UserDTOs.Count());
        Assert.Equal(usersWithCountDTO.UserDTOs.ElementAt(0).FirstName, users[0].FirstName);
        Assert.Equal(usersWithCountDTO.UserDTOs.ElementAt(0).LastName, users[0].LastName);
        Assert.Equal(usersWithCountDTO.UserDTOs.ElementAt(1).FirstName, users[1].FirstName);
        Assert.Equal(usersWithCountDTO.UserDTOs.ElementAt(1).LastName, users[1].LastName);
    }

    [Fact]
    public async Task GetUsersAsync_WhenNoUsersAreFound_ReturnsUsersWithCountDTO_WithEmptyCollectionAndTotalCountZero()
    {
        // Arrange
        var query = new UserQuery(null, null, null, null);

        List<User> users = [];
        int totalCount = users.Count;

        _userRepositoryMock.Setup(x => x.GetUsersAsync(query)).ReturnsAsync((users, totalCount));

        // Act
        var result = await _userService.GetUsersAsync(query);

        // Assert
        var userWithCountDTO = Assert.IsType<UsersWithCountDTO>(result);
        Assert.Empty(userWithCountDTO.UserDTOs);
        Assert.Equal(0, userWithCountDTO.TotalCount);
    }

    #endregion GetUsersAsync

    #region GetUserByIdAsync

    [Fact]
    public async Task GetUserByIdAsync_WhenUserIsFound_ReturnsUserDTO()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User { Id = new UserId(id), FirstName = "Peter" };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(id);

        // Assert
        var userDTO = Assert.IsType<UserDTO>(result);
        Assert.Equal(user.Id, userDTO.Id);
        Assert.Equal(user.FirstName, userDTO.FirstName);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserIsNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = new User { Id = new UserId(id), FirstName = "Peter" };

        _userRepositoryMock.Setup(x => x.GetUserByIdAsync(user.Id))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(id);

        // Assert
        Assert.Null(result);
    }

    #endregion GetUserByIdAsync

    #region UpdateuserAsync

    [Fact]
    public async Task UpdateUserAsync_WhenUserWasUpdated_ReturnsUserDTO() 
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);

        var userUpdateDTO = new UserUpdateDTO("Tom", "Jerryson", "61524234", "tom@planetearth.com");

        var user = new User
        {
            Id = userId,
            FirstName = "Tom",
            LastName = "Jerryson",
            PhoneNumber = "61524234",
            Email = "tom@planetearth.com",
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(x => x.UpdateUserAsync(user.Id, It.IsAny<User>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.UpdateUserAsync(id, userUpdateDTO);

        // Assert
        Assert.IsType<UserDTO>(result);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserWasNotUpdated_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);

        var userUpdateDTO = new UserUpdateDTO("Tom", "Jerryson", "61524234", "tom@planetearth.com");

        var user = new User
        {
            Id = userId,
            FirstName = "Tom",
            LastName = "Jerryson",
            PhoneNumber = "61524234",
            Email = "tom@planetearth.com",
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(x => x.UpdateUserAsync(user.Id, It.IsAny<User>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateUserAsync(id, userUpdateDTO);

        // Assert
        Assert.Null(result);
    }


    #endregion UpdateuserAsync

    #region DeleteUserAsync

    [Fact]
    public async Task DeleteUserAsync_WhenUserWasDeleted_ReturnsUserDTO()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = new UserId(id);

        var user = new User
        {
            Id = userId,
            FirstName = "Tom",
            LastName = "Jerryson",
            PhoneNumber = "61524234",
            Email = "tom@planetearth.com",
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(x => x.DeleteUserAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.DeleteUserAsync(id);

        // Assert
        var userDTO = Assert.IsType<UserDTO>(result);
        Assert.Equal(user.Id, userDTO.Id);
        Assert.Equal(user.FirstName, userDTO.FirstName);
        Assert.Equal(user.LastName, userDTO.LastName);
        Assert.Equal(user.PhoneNumber, userDTO.PhoneNumber);
        Assert.Equal(user.Email, userDTO.Email);
    }

    #endregion DeleteUserAsync
}
