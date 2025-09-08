using Legno.Application.Dtos.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IPartnerService
    {
        Task<BusinessServiceDto> AddBusinessServiceAsync(CreateBusinessServiceDto createDto);
        Task<BusinessServiceDto?> GetBusinessServiceAsync(string id);
        Task<List<BusinessServiceDto>> GetAllBusinessServicesAsync();
        Task<BusinessServiceDto> UpdateBusinessServiceAsync(UpdateBusinessServiceDto updateDto);
        Task DeleteBusinessServiceAsync(string id);
    }
}
