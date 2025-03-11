using GetARoomAPI.Features.HATEOAS;
using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Models.DTOs.UserDTOs;

public record UserDTO
(
    UserId Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    List<Link> Links
);
