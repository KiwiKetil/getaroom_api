using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Models.DTOs;

public record UserDTO
(
    UserId Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    List<Link> Links
);

