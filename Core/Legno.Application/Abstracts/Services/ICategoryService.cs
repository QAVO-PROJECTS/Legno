using Legno.Application.Dtos.Category;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> AddCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto?> GetCategoryAsync(string categoryId);
        Task<List<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto);
        Task DeleteCategoryAsync(string categoryId);
    }
}
