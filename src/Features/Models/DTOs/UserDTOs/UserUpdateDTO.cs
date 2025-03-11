namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record UserUpdateDTO
(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email
);
