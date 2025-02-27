namespace RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;

public record UsersAndCountDTO
(
    int TotalCount, 
    IEnumerable<UserDTO> UserDTOs
);

