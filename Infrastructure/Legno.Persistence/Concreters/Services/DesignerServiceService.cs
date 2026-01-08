using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.BusinessService;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class DesignerServiceService : IDesignerServiceService
    {
        private readonly IDesignerServiceReadRepository _read;
        private readonly IDesignerServiceWriteRepository _write;
        private readonly IFileService _fileService; // 🔁 Cloudinary əvəzinə
        private readonly IMapper _mapper;

        public DesignerServiceService(
            IDesignerServiceReadRepository read,
            IDesignerServiceWriteRepository write,
            IFileService fileService,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _fileService = fileService;
            _mapper = mapper;
        }

        // ───────────────────────────────
        // ✅ Yeni xidmət əlavə et
        // ───────────────────────────────
        public async Task<BusinessServiceDto> AddBusinessServiceAsync(CreateBusinessServiceDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<DesignerService>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            // 📂 Şəkil yüklə (mütləq tələb olunur)
            if (dto.CardImage == null)
                throw new GlobalAppException("Kart şəkli tələb olunur.");

            entity.CardImage = await _fileService.UploadFile(dto.CardImage, "designer-services");

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Tək xidməti gətir
        // ───────────────────────────────
        public async Task<BusinessServiceDto?> GetBusinessServiceAsync(string businessServiceId)
        {
            if (!Guid.TryParse(businessServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                EnableTraking: false
            );

            if (entity == null)
                throw new GlobalAppException("Xidmət tapılmadı.");

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Bütün xidmətləri gətir
        // ───────────────────────────────
        public async Task<List<BusinessServiceDto>> GetAllCategoriesAsync()
        {
            var list = await _read.GetAllAsync(
                x => !x.IsDeleted,
                EnableTraking: false,
                orderBy: q => q.OrderBy(x => x.CreatedDate)
            );

            return list.Select(_mapper.Map<BusinessServiceDto>).ToList();
        }

        // ───────────────────────────────
        // ✅ Xidməti yenilə
        // ───────────────────────────────
        public async Task<BusinessServiceDto> UpdateBusinessServiceAsync(UpdateBusinessServiceDto dto)
        {
            if (dto == null || !Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // 🔤 Text sahələri yenilə (mapper null gələnləri toxunmur)
            _mapper.Map(dto, entity);

            // 📂 Yeni şəkil yüklənibsə, köhnəsini sil
            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _fileService.DeleteFile("designer-services", entity.CardImage);

                entity.CardImage = await _fileService.UploadFile(dto.CardImage, "designer-services");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Xidməti sil (soft delete)
        // ───────────────────────────────
        public async Task DeleteBusinessServiceAsync(string businessServiceId)
        {
            if (!Guid.TryParse(businessServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // 📂 Əgər şəkil varsa, faylı da sil
            if (!string.IsNullOrWhiteSpace(entity.CardImage))
                await _fileService.DeleteFile("designer-services", entity.CardImage);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
