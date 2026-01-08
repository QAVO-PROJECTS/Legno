using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Fabric;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Application.Abstracts.Repositories;
using Microsoft.AspNetCore.Http;
using Legno.Application.Absrtacts.Services;

namespace Legno.Persistence.Concreters.Services
{
    public class FabricService : IFabricService
    {
        private readonly IFabricReadRepository _read;
        private readonly IFabricWriteRepository _write;
        private readonly IFileService _fileService; // 🔁 Cloudinary əvəzinə

        public FabricService(
            IFabricReadRepository read,
            IFabricWriteRepository write,
            IFileService fileService)
        {
            _read = read;
            _write = write;
            _fileService = fileService;
        }

        // ───────────────────────────────
        // ✅ Yeni material (şəkil) əlavə et
        // ───────────────────────────────
        public async Task<FabricDto> AddFabricServiceAsync(IFormFile fabricFile)
        {
            if (fabricFile == null || fabricFile.Length == 0)
                throw new GlobalAppException("Material şəkli göndərilməyib.");

            // 📂 Faylı serverə yüklə
            var fileName = await _fileService.UploadFile(fabricFile, "fabrics");

            var entity = new Fabric
            {
                Id = Guid.NewGuid(),
                Image = fileName,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return new FabricDto
            {
                Id = entity.Id.ToString(),
                Image = entity.Image
            };
        }

        // ───────────────────────────────
        // ✅ Tək materialı gətir
        // ───────────────────────────────
        public async Task<FabricDto?> GetFabricServiceAsync(string fabricId)
        {
            if (!Guid.TryParse(fabricId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                EnableTraking: false
            ) ?? throw new GlobalAppException("Material tapılmadı.");

            return new FabricDto
            {
                Id = entity.Id.ToString(),
                Image = entity.Image
            };
        }

        // ───────────────────────────────
        // ✅ Bütün materialları gətir
        // ───────────────────────────────
        public async Task<List<FabricDto>> GetAllFabricsAsync()
        {
            var list = await _read.GetAllAsync(
                x => !x.IsDeleted,
                EnableTraking: false,
                orderBy: q => q.OrderBy(x => x.CreatedDate)
            );

            return list.Select(x => new FabricDto
            {
                Id = x.Id.ToString(),
                Image = x.Image
            }).ToList();
        }

        // ───────────────────────────────
        // ✅ Material sil (soft delete + faylı sil)
        // ───────────────────────────────
        public async Task DeleteFabricServiceAsync(string fabricId)
        {
            if (!Guid.TryParse(fabricId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Material tapılmadı.");

            // 📂 Əgər fayl varsa — sil
            if (!string.IsNullOrWhiteSpace(entity.Image))
                await _fileService.DeleteFile("fabrics", entity.Image);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
