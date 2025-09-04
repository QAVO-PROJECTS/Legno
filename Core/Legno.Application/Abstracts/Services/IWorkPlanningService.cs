using Legno.Application.Dtos.CommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IWorkPlanningService
    {
        Task<CommonServiceDto> AddCommonServiceAsync(CreateCommonServiceDto createDto);
        Task<CommonServiceDto?> GetCommonServiceAsync(string id);
        Task<List<CommonServiceDto>> GetAllCommonServicesAsync();
        Task<CommonServiceDto> UpdateCommonServiceAsync(UpdateCommonServiceDto updateDto);
        Task DeleteCommonServiceAsync(string id);
    }
}
