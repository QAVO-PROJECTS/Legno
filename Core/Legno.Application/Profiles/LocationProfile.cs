using AutoMapper;
using Legno.Application.Dtos.Location;
using Legno.Domain.Entities;

namespace Legno.Application.Profiles
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            // Entity -> DTO
            CreateMap<Location, LocationDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));

            // Create DTO -> Entity
            CreateMap<CreateLocationDto, Location>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
                .ForMember(d => d.DeletedDate, o => o.Ignore());

            // Update DTO -> Entity (null-lar toxunulmur)
            CreateMap<UpdateLocationDto, Location>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
