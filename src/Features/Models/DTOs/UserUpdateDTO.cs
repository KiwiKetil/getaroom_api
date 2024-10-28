namespace RoomSchedulerAPI.Features.Models.DTOs;

public record UserUpdateDTO
(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email
);
