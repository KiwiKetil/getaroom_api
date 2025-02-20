using AutoMapper;
using RoomSchedulerAPI.Features.HateOAS;
using RoomSchedulerAPI.Features.Models.DTOs.UserDTOs;
using RoomSchedulerAPI.Features.Models.Entities;
using System.Globalization;

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

        CreateMap<UserUpdateDTO, User>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src =>
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(src.FirstName.ToLowerInvariant() ?? string.Empty)))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src =>
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(src.LastName.ToLowerInvariant() ?? string.Empty)));        

        CreateMap<UserRegistrationDTO, User>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src =>
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(src.FirstName.ToLowerInvariant() ?? string.Empty)))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src =>
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(src.LastName.ToLowerInvariant() ?? string.Empty)));
    }
}
