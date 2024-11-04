using AutoMapper;
using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.DTOs;
using RoomSchedulerAPI.Features.Models.Entities;

namespace RoomSchedulerAPI.Features.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserDTO, User>();

        CreateMap<User, UserDTO>()
         .ForMember(dest => dest.Links, opt => opt.MapFrom(src => new List<Link>
         {
            new($"/api/v1/users/{src.Id}", "self", "GET"),
            new($"/api/v1/users/{src.Id}", "update", "PUT"),
            new($"/api/v1/users/{src.Id}", "delete", "DELETE")
         }))
         .ConstructUsing(src => new UserDTO(
            src.Id,
            src.FirstName ?? string.Empty,
            src.LastName ?? string.Empty,
            src.PhoneNumber ?? string.Empty,
            src.Email ?? string.Empty,
            new List<Link>()
         ));
    }
}
