namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record UsersWithCountDTO
(
    int TotalCount,
    IEnumerable<UserDTO> UserDTOs
);

