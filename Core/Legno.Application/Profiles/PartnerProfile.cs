using AutoMapper;
using Legno.Application.Dtos.BusinessService;
using Legno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Profiles
{
    public class PartnerProfile : Profile
    {
        public PartnerProfile()
        {
            CreateMap<Partner, BusinessServiceDto>()
        .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));
   


            CreateMap<CreateBusinessServiceDto, Partner>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
                .ForMember(d => d.DeletedDate, o => o.Ignore())
                .ForMember(d => d.CardImage, o => o.Ignore());


            CreateMap<UpdateBusinessServiceDto, Partner>()
                .ForMember(d => d.CardImage, o => o.Ignore())

                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
