using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.ServiceSlider;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    internal class ServiceSliderService: IServiceSliderService
    {
        private readonly IServiceSliderReadRepository _read;
        private readonly IServiceSliderWriteRepository _write;
        private readonly CloudinaryService _cloudinary;

        public ServiceSliderService(IServiceSliderReadRepository read,
                             IServiceSliderWriteRepository write,
                             CloudinaryService cloudinary)
        {
            _read = read;
            _write = write;
            _cloudinary = cloudinary;
        }

        public async Task<ServiceSliderDto> AddServiceSliderAsync(IFormFile ServiceSliderName)
        {
            if (ServiceSliderName == null || ServiceSliderName.Length == 0)
                throw new GlobalAppException("Şəkil faylı göndərilməyib.");

            var url = await _cloudinary.UploadFileAsync(ServiceSliderName);

            var entity = new ServiceSlider
            {
                Id = Guid.NewGuid(),
                ServiceSliderImage = url,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return new ServiceSliderDto { Id = entity.Id.ToString(), Image = entity.ServiceSliderImage };
        }

        public async Task<ServiceSliderDto?> GetServiceSliderAsync(string ServiceSliderId)
        {
            if (!Guid.TryParse(ServiceSliderId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: false);
            if (entity == null) throw new GlobalAppException("Material tapılmadı.");

            return new ServiceSliderDto { Id = entity.Id.ToString(), Image = entity.ServiceSliderImage };
        }

        public async Task<List<ServiceSliderDto>> GetAllServiceSlidersAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted, EnableTraking: false,
                                               orderBy: q => q.OrderBy(x => x.CreatedDate));

            return list.Select(x => new ServiceSliderDto
            {
                Id = x.Id.ToString(),
                Image = x.ServiceSliderImage
            }).ToList();
        }

        public async Task DeleteServiceSliderAsync(string ServiceSliderId)
        {
            if (!Guid.TryParse(ServiceSliderId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Material tapılmadı.");

            if (!string.IsNullOrWhiteSpace(entity.ServiceSliderImage))
                await _cloudinary.DeleteFileAsync(entity.ServiceSliderImage);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
