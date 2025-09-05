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
    public class DesignerServiceProfile : Profile
    {
        public DesignerServiceProfile()
        {
            // Entity -> DTO
            CreateMap<DesignerService, BusinessServiceDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id.ToString()));

            // Create DTO -> Entity (CardImage faylını service dolduracaq)
            CreateMap<CreateBusinessServiceDto, DesignerService>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CardImage, o => o.Ignore())
                .ForMember(d => d.IsDeleted, o => o.Ignore())
                .ForMember(d => d.CreatedDate, o => o.Ignore())
                .ForMember(d => d.LastUpdatedDate, o => o.Ignore())
                .ForMember(d => d.DeletedDate, o => o.Ignore());

            // Update DTO -> Entity (null gələn sahələrə toxunmamaq üçün)
            CreateMap<UpdateBusinessServiceDto, DesignerService>()
                .ForMember(d => d.CardImage, o => o.Ignore()) // faylı service yeniləyəcək
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
