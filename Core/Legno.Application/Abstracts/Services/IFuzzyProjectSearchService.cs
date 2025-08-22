
using Legno.Application.Dtos.Project;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IFuzzyProjectSearchService
    {
        Task<List<ProjectDto>> SearchAsync(string query);
    }
}
