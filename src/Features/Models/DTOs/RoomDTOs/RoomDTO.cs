using GetARoomAPI.Features.HATEOAS;
using GetARoomAPI.Features.Models.Entities;

namespace GetARoomAPI.Features.Models.DTOs.RoomDTOs;

public record RoomDTO
(
    RoomId Id,
    string RoomName,
    sbyte Capacity,
    bool MonitorAvailable,
    List<Link> Links
);