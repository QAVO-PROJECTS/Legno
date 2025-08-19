using AutoMapper;
using Legno.Application.Dtos.Category;
using Legno.Domain.Entities;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.IsDeleted, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.LastUpdatedDate, opt => opt.Ignore())
            .ForMember(d => d.DeletedDate, opt => opt.Ignore());

        CreateMap<UpdateCategoryDto, Category>()
         
            .ForMember(d => d.LastUpdatedDate, opt => opt.Ignore());
    }
}
