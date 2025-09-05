using Legno.Application.Dtos.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IBusinessServiceService
    {
        Task<BusinessServiceDto> AddBusinessServiceAsync(CreateBusinessServiceDto dto);
        Task<BusinessServiceDto?> GetBusinessServiceAsync(string BusinessServiceId);
        Task<List<BusinessServiceDto>> GetAllCategoriesAsync();
        Task<BusinessServiceDto> UpdateBusinessServiceAsync(UpdateBusinessServiceDto dto);
        Task DeleteBusinessServiceAsync(string BusinessServiceId);
    }
}
