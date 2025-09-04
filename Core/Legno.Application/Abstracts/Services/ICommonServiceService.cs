using Legno.Application.Dtos.CommonService;

namespace Legno.Application.Abstracts.Services
{
    public interface ICommonServiceService
    {
        Task<CommonServiceDto> AddCommonServiceAsync(CreateCommonServiceDto createDto);
        Task<CommonServiceDto?> GetCommonServiceAsync(string id);
        Task<List<CommonServiceDto>> GetAllCommonServicesAsync();
        Task<CommonServiceDto> UpdateCommonServiceAsync(UpdateCommonServiceDto updateDto);
        Task DeleteCommonServiceAsync(string id);
    }
}
