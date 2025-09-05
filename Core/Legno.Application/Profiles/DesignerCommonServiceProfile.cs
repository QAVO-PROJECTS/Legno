using AutoMapper;
using Legno.Application.Dtos.DesignerCommonService;
using Legno.Domain.Entities;

namespace Legno.Application.Profiles
{
    public class DesignerCommonServiceProfile : Profile
    {
        public DesignerCommonServiceProfile()
        {
            // Entity -> DTO
            CreateMap<DesignerCommonService, DesignerCommonServiceDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));

            // Create DTO -> Entity (CardImage faylı service-də yüklənəcək)
            CreateMap<CreateDesignerCommonServiceDto, DesignerCommonService>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CardImage, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
                .ForMember(d => d.DeletedDate, o => o.Ignore());

            // Update DTO -> Entity (CardImage faylını service idarə edir)
            CreateMap<UpdateDesignerCommonServiceDto, DesignerCommonService>()
                .ForMember(d => d.CardImage, o => o.Ignore());
        }
    }
}
