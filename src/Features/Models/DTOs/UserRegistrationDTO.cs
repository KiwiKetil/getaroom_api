using System.Security.Policy;

namespace RoomSchedulerAPI.Features.Models.DTOs;

public record UserRegistrationDTO
(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password
);
