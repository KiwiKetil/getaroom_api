namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record CompleteUserDTO
(
    int TotalCount,
    IEnumerable<UserDTO> UserDTOs
);

