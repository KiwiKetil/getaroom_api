using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Models.DTOs;

public record RoomReservationDTO
(
    RoomreservationId Id,
    string UserFirstName,
    string UserLastName,
    string RoomName,
    DateTime StartTime,
    DateTime EndTime,
    List<Link> Links
);