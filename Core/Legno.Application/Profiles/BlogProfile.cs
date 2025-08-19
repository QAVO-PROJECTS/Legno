using AutoMapper;
using Legno.Application.Dtos.Blog;
using Legno.Domain.Entities;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<Blog, BlogDto>()
            .ForMember(d => d.CreatedDate,
                opt => opt.MapFrom(s => s.CreatedDate.AddHours(4).ToString("dd.MM.yyyy HH:mm")));

        CreateMap<CreateBlogDto, Blog>()
            .ForMember(d => d.BlogImage, opt => opt.Ignore())
            .ForMember(d => d.AuthorImage, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.IsDeleted, opt => opt.Ignore());

        CreateMap<UpdateBlogDto, Blog>()
         
            .ForMember(d => d.BlogImage, opt => opt.Ignore())
            .ForMember(d => d.AuthorImage, opt => opt.Ignore());
    }
}
