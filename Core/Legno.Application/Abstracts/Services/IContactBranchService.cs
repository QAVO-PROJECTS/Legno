using Legno.Application.Dtos.ContactBranch;

namespace Legno.Application.Abstracts.Services
{
    public interface IContactBranchService
    {
        Task<ContactBranchDto> CreateAsync(CreateContactBranchDto dto);          // :contentReference[oaicite:1]{index=1}
        Task<ContactBranchDto?> GetAsync(string id);
        Task<List<ContactBranchDto>> GetAllAsync();

        Task<ContactBranchDto> UpdateAsync(UpdateContactBranchDto dto);          // :contentReference[oaicite:2]{index=2}
        Task DeleteAsync(string id);
    }
}