using AutoMapper;
using Legno.Application.Dtos.Purchase;
using Legno.Domain.Entities;

namespace Legno.Persistence.MapperProfiles
{
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile()
        {
            CreateMap<CreatePurchaseDto, Purchase>();
            CreateMap<Purchase, PurchaseDto>();
            CreateMap<UpdatePurchaseDtos, Purchase>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
