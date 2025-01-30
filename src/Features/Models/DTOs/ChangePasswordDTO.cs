namespace RoomSchedulerAPI.Features.Models.DTOs;

public record ChangePasswordDTO
(
    string Email,
    string CurrentPassword,
    string NewPassword
);
