namespace RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;

public record ChangePasswordDTO
(
    string Email,
    string CurrentPassword,
    string NewPassword
);
