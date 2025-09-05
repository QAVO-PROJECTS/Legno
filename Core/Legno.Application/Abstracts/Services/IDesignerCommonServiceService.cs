using Legno.Application.Dtos.DesignerCommonService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IDesignerCommonServiceService
    {
        Task<DesignerCommonServiceDto> AddDesignerCommonServiceAsync(CreateDesignerCommonServiceDto dto);
        Task<DesignerCommonServiceDto?> GetDesignerCommonServiceAsync(string designerCommonServiceId);
        Task<List<DesignerCommonServiceDto>> GetAllCategoriesAsync();
        Task<DesignerCommonServiceDto> UpdateDesignerCommonServiceAsync(UpdateDesignerCommonServiceDto dto);
        Task DeleteDesignerCommonServiceAsync(string designerCommonServiceId);
    }
}
