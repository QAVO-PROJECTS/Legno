using AutoMapper;
using Legno.Application.Dtos.Career;
using Legno.Domain.Entities;

namespace Legno.Application.Mapping
{
    public class CareerProfile : Profile
    {
        public CareerProfile()
        {
            CreateMap<Career, CareerDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));

            CreateMap<CreateCareerDto, Career>()
                .ForMember(d => d.FileName, o => o.Ignore());

            CreateMap<UpdateCareerDto, Career>()
                .ForAllMembers(o =>
                    o.Condition((src, dest, val) => val != null));
        }
    }
}
