using Legno.Application.Dtos.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IBlogService
    {
        Task<BlogDto> AddBlogAsync(CreateBlogDto createBlogDto);
        Task<BlogDto?> GetBlogAsync(string BlogId);
        Task<List<BlogDto>> GetAllBlogsAsync();
        Task<BlogDto> UpdateBlogAsync(UpdateBlogDto updateBlogDto);
     
        Task DeleteBlogAsync(string BlogId);
    }
}
