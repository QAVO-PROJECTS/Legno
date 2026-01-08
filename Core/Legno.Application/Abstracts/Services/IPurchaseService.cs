using Legno.Application.Dtos.Purchase;

namespace Legno.Application.Abstracts.Services
{
    public interface IPurchaseService
    {
        Task<PurchaseDto> AddPurchaseAsync(CreatePurchaseDto dto);
        Task<PurchaseDto?> GetPurchaseAsync(string id);
        Task<List<PurchaseDto>> GetAllPurchasesAsync();
        Task<PurchaseDto> UpdatePurchaseAsync(UpdatePurchaseDtos dto);
        Task DeletePurchaseAsync(string id);
    }
}
