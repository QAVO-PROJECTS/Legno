using AutoMapper;
using Legno.Application.Dtos.Setting;
using Legno.Domain.Entities;

namespace Legno.Application.Profiles
{
    public class SettingProfile : Profile
    {
        public SettingProfile()
        {
            CreateMap<CreateSettingDto, Setting>();

            CreateMap<Setting, SettingDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.ToString()));

            // Update mapping OPTIONAL — (biz update-də manual edəcəyik)
            CreateMap<UpdateSettingDto, Setting>();
        }
    }
}