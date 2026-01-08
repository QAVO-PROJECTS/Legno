using AutoMapper;
using Legno.Application.Dtos.Article;
using Legno.Domain.Entities;

namespace Legno.Application.Mapping
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleDto>()
                .ForMember(d => d.Images,
                    o => o.MapFrom(s => s.Images.Select(x => x.Name)));

            CreateMap<CreateArticleDto, Article>()
                .ForMember(d => d.Images, o => o.Ignore());

            CreateMap<UpdateArticleDto, Article>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, member) => member != null));
        }
    }
}
