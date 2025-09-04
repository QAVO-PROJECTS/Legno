using AutoMapper;
using Legno.Application.Dtos.CommonService;
using Legno.Domain.Entities;

namespace Legno.Application.Profiles
{
    public class CommonServiceProfile : Profile
    {
        public CommonServiceProfile()
        {
            CreateMap<CommonService, CommonServiceDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));

            CreateMap<CreateCommonServiceDto, CommonService>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
                .ForMember(d => d.DeletedDate, o => o.Ignore());

            CreateMap<UpdateCommonServiceDto, CommonService>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
