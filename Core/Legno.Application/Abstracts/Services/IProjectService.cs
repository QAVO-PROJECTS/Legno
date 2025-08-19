using Legno.Application.Dtos.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IProjectService
    {
        Task<ProjectDto> AddProjectAsync(CreateProjectDto dto);
        Task<ProjectDto?> GetProjectAsync(string projectId);
        Task<List<ProjectDto>> GetAllProjectsAsync(); // DisplayOrderId ASC qaytaraq
        Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto);
        Task DeleteProjectAsync(string projectId);

        // Kütləvi reorder: (Id, DisplayOrderId) siyahısını verir və hamısını yeniləyir
        Task BulkReorderAsync(List<ProjectReorderDto> items);
        Task<List<ProjectDto>> GetProjectsByCategoryAsync(string categoryId);
    }
}
