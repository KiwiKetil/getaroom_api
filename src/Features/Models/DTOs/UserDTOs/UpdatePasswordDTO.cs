namespace RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;

public record UpdatePasswordDTO
(
    string Email,
    string CurrentPassword,
    string NewPassword
);
