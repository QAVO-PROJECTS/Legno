using AutoMapper;
using Legno.Application.Dtos.Announcement;
using Legno.Domain.Entities;

namespace Legno.Application.Mapping
{
    public class AnnouncementProfile : Profile
    {
        public AnnouncementProfile()
        {
            // Entity -> DTO
            CreateMap<Announcement, AnnouncementDto>();


            // Create DTO -> Entity
            CreateMap<CreateAnnouncementDto, Announcement>()
                .ForMember(d => d.AuthorImage, o => o.Ignore())
                .ForMember(d => d.CardImage, o => o.Ignore());

            // Update DTO -> Entity (yalnız dolu sahələr)
            CreateMap<UpdateAnnouncementDto, Announcement>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) =>
                        srcMember != null));
        }
    }
}
