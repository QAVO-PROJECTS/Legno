using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Fabric;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Application.Abstracts.Repositories;
using Microsoft.AspNetCore.Http;

namespace Legno.Persistence.Concreters.Services
{
    public class FabricService : IFabricService
    {
        private readonly IFabricReadRepository _read;
        private readonly IFabricWriteRepository _write;
        private readonly CloudinaryService _cloudinary;

        public FabricService(IFabricReadRepository read,
                             IFabricWriteRepository write,
                             CloudinaryService cloudinary)
        {
            _read = read;
            _write = write;
            _cloudinary = cloudinary;
        }

        public async Task<FabricDto> AddFabricServiceAsync(IFormFile fabricName)
        {
            if (fabricName == null || fabricName.Length == 0)
                throw new GlobalAppException("Şəkil faylı göndərilməyib.");

            var url = await _cloudinary.UploadFileAsync(fabricName);

            var entity = new Fabric
            {
                Id = Guid.NewGuid(),
                Image = url,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return new FabricDto { Id = entity.Id.ToString(), Image = entity.Image };
        }

        public async Task<FabricDto?> GetFabricServiceAsync(string fabricId)
        {
            if (!Guid.TryParse(fabricId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: false);
            if (entity == null) throw new GlobalAppException("Material tapılmadı.");

            return new FabricDto { Id = entity.Id.ToString(), Image = entity.Image };
        }

        public async Task<List<FabricDto>> GetAllFabricsAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted, EnableTraking: false,
                                               orderBy: q => q.OrderBy(x => x.CreatedDate));

            return list.Select(x => new FabricDto
            {
                Id = x.Id.ToString(),
                Image = x.Image
            }).ToList();
        }

        public async Task DeleteFabricServiceAsync(string fabricId)
        {
            if (!Guid.TryParse(fabricId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Material tapılmadı.");

            if (!string.IsNullOrWhiteSpace(entity.Image))
                await _cloudinary.DeleteFileAsync(entity.Image);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
