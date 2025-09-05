using AutoMapper;
using Legno.Application.Dtos.BusinessService;
using Legno.Domain.Entities;

namespace Legno.Application.Profiles
{
    public class BusinessServiceProfile : Profile
    {
        public BusinessServiceProfile()
        {
            // Entity -> DTO
            CreateMap<BusinessService, BusinessServiceDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));

            // Create DTO -> Entity (CardImage faylını service dolduracaq)
            CreateMap<CreateBusinessServiceDto, BusinessService>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CardImage, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
                .ForMember(d => d.DeletedDate, o => o.Ignore());

            // Update DTO -> Entity (null gələn sahələrə toxunmamaq üçün)
            CreateMap<UpdateBusinessServiceDto, BusinessService>()
                .ForMember(d => d.CardImage, o => o.Ignore()) // faylı service yeniləyəcək
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
