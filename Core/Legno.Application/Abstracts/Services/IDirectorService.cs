using Legno.Application.Dtos.Director;

namespace Legno.Application.Abstracts.Services
{
    public interface IDirectorService
    {
        Task<DirectorDto> AddDirectorAsync(CreateDirectorDto dto);
        Task<DirectorDto?> GetDirectorAsync(string id);
        Task<List<DirectorDto>> GetAllDirectorsAsync();
        Task<DirectorDto> UpdateDirectorAsync(UpdateDirectorDto dto);
        Task DeleteDirectorAsync(string id);
    }
}