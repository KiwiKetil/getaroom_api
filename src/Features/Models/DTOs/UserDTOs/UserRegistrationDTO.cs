namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record UserRegistrationDTO
(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password
);
