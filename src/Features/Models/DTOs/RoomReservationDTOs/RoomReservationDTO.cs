using GetARoomAPI.Features.HATEOAS;
using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Models.DTOs.RoomReservationDTOs;

public record RoomReservationDTO
(
    RoomReservationId Id,
    string UserFirstName,
    string UserLastName,
    string RoomName,
    DateTime StartTime,
    DateTime EndTime,
    List<Link> Links
);