using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Models.DTOs.RoomDTOs;

public record RoomDTO
(
    RoomId Id,
    string RoomName,
    sbyte Capacity,
    bool MonitorAvailable,
    List<Link> Links
);