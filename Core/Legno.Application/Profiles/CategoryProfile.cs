using AutoMapper;
using Legno.Application.Dtos.Category;
using Legno.Domain.Entities;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>()
                      .ForMember(d => d.CategorySliderImages,
              o => o.MapFrom(s => s.CategorySliderImages == null
                  ? null
                  : s.CategorySliderImages.Where(i => !i.IsDeleted).Select(i => i.Name)))
            ;
        CreateMap<CreateCategoryDto, Category>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.IsDeleted, opt => opt.Ignore())
            .ForMember(d => d.CreatedDate, opt => opt.Ignore())
            .ForMember(d => d.LastUpdatedDate, opt => opt.Ignore())
            .ForMember(d => d.DeletedDate, opt => opt.Ignore())
                    .ForMember(d => d.CategorySliderImages, o => o.Ignore());

        CreateMap<UpdateCategoryDto, Category>()
         
            .ForMember(d => d.LastUpdatedDate, opt => opt.Ignore())
            .ForMember(d => d.CategorySliderImages, o => o.Ignore());
    }
}
