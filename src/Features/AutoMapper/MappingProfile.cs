using AutoMapper;
using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDTO>()
         .ConstructUsing(src => new UserDTO(
            src.Id,
            src.FirstName ?? string.Empty,
            src.LastName ?? string.Empty,
            src.PhoneNumber ?? string.Empty,
            src.Email ?? string.Empty,
            new List<Link>()
         ))
         .ForMember(dest => dest.Links, opt => opt.MapFrom(src => new List<Link>
         {
            new($"/api/v1/users/{src.Id.Value}", "self", "GET"),
            new($"/api/v1/users/{src.Id.Value}", "update", "PUT"),
            new($"/api/v1/users/{src.Id.Value}", "delete", "DELETE")
         }));
    }
}
