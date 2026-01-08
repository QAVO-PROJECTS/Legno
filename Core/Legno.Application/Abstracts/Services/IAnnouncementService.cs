using Legno.Application.Dtos.Announcement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface IAnnouncementService
    {
        Task<AnnouncementDto> AddAnnouncementAsync(CreateAnnouncementDto createDto);
        Task<AnnouncementDto?> GetAnnouncementAsync(string id);
        Task<List<AnnouncementDto>> GetAllAnnouncementsAsync();
        Task<AnnouncementDto> UpdateAnnouncementAsync(UpdateAnnouncementDto updateDto);
        Task DeleteAnnouncementAsync(string id);
    }
}
