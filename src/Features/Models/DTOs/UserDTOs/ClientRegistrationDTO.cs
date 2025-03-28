namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record ClientRegistrationDTO
(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password
);
