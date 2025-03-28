using GetARoomAPI.Features.HATEOAS;

namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record EmployeeRegistrationDTO
(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email
);
