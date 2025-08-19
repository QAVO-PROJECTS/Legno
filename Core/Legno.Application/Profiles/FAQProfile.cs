using AutoMapper;
using Legno.Application.Dtos.FAQ;
using Legno.Domain.Entities;

public class FAQProfile : Profile
{
    public FAQProfile()
    {
        // Entity -> DTO
        CreateMap<FAQ, FAQDto>();

        // CreateDTO -> Entity
        CreateMap<CreateFAQDto, FAQ>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.IsDeleted, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.LastUpdatedDate, opt => opt.Ignore())
            .ForMember(d => d.DeletedDate, opt => opt.Ignore());

        // UpdateDTO -> Entity (yalnız null olmayanlar kopyalansın)
        CreateMap<UpdateFAQDto, FAQ>()
        
            .ForMember(d => d.LastUpdatedDate, opt => opt.Ignore());
    }
}
