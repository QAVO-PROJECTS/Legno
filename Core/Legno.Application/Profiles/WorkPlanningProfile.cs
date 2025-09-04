using AutoMapper;
using Legno.Application.Dtos.CommonService;
using Legno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Profiles
{
    public class WorkPlanningProfile:Profile
    {
        public WorkPlanningProfile()
        {
            CreateMap<WorkPlanning, CommonServiceDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));

            CreateMap<CreateCommonServiceDto, WorkPlanning>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
                .ForMember(d => d.DeletedDate, o => o.Ignore());

            CreateMap<UpdateCommonServiceDto, WorkPlanning>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
