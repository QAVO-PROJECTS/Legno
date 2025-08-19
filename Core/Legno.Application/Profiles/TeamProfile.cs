using AutoMapper;
using Legno.Application.Dtos.Team;
using Legno.Domain.Entities;

namespace Legno.Application.Mapping
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            // Domain -> DTO
            CreateMap<Team, TeamDto>();

            // Create DTO -> Domain
            CreateMap<CreateTeamDto, Team>()
                .ForMember(d => d.CardImage, opt => opt.Ignore())      // fayl upload sonrası setlənəcək
                .ForMember(d => d.DisplayOrderId, opt => opt.Ignore()) // auto-increment service-də veriləcək
                .ForMember(d => d.IsDeleted, opt => opt.Ignore());

            // Update DTO -> Domain (yalnız null olmayanları kopyala)

            CreateMap<UpdateTeamDto, Team>()
                 .ForMember(d => d.CardImage, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
               // fayl varsa service-də dəyişəcəyik
        }
    }
}
