using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.ServiceSlider;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Legno.Persistence.Concreters.Services
{
    public class ServiceSliderService : IServiceSliderService
    {
        private readonly IServiceSliderReadRepository _read;
        private readonly IServiceSliderWriteRepository _write;
        private readonly IFileService _fileService;

        public ServiceSliderService(
            IServiceSliderReadRepository read,
            IServiceSliderWriteRepository write,
            IFileService fileService)
        {
            _read = read;
            _write = write;
            _fileService = fileService;
        }

        public async Task<ServiceSliderDto> AddServiceSliderAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new GlobalAppException("Şəkil faylı göndərilməyib.");

            var fileName = await _fileService.UploadFile(image, "service-sliders");

            var entity = new ServiceSlider
            {
                Id = Guid.NewGuid(),
                ServiceSliderImage = fileName,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return new ServiceSliderDto { Id = entity.Id.ToString(), Image = entity.ServiceSliderImage };
        }

        public async Task<ServiceSliderDto?> GetServiceSliderAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted, EnableTraking: false)
                ?? throw new GlobalAppException("Slider tapılmadı.");

            return new ServiceSliderDto { Id = entity.Id.ToString(), Image = entity.ServiceSliderImage };
        }

        public async Task<List<ServiceSliderDto>> GetAllServiceSlidersAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted,
                orderBy: q => q.OrderBy(x => x.CreatedDate),
                EnableTraking: false);

            return list.Select(x => new ServiceSliderDto
            {
                Id = x.Id.ToString(),
                Image = x.ServiceSliderImage
            }).ToList();
        }

        public async Task DeleteServiceSliderAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted, EnableTraking: true)
                ?? throw new GlobalAppException("Slider tapılmadı.");

            if (!string.IsNullOrWhiteSpace(entity.ServiceSliderImage))
                await _fileService.DeleteFile("service-sliders", entity.ServiceSliderImage);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
