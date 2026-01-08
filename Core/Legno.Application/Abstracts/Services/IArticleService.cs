using Legno.Application.Dtos.Article;

namespace Legno.Application.Abstracts.Services
{
    public interface IArticleService
    {
        Task<ArticleDto> AddArticleAsync(CreateArticleDto dto);
        Task<ArticleDto?> GetArticleAsync(string id);
        Task<List<ArticleDto>> GetAllArticlesAsync();
        Task<ArticleDto> UpdateArticleAsync(UpdateArticleDto dto);
        Task DeleteArticleAsync(string id);
    }
}
