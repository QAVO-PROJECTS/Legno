using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.DesignerCommonService;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class DesignerCommonServiceService : IDesignerCommonServiceService
    {
        private readonly IDesignerCommonServiceReadRepository _read;
        private readonly IDesignerCommonServiceWriteRepository _write;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public DesignerCommonServiceService(
            IDesignerCommonServiceReadRepository read,
            IDesignerCommonServiceWriteRepository write,
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
        public async Task<DesignerCommonServiceDto> AddDesignerCommonServiceAsync(CreateDesignerCommonServiceDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<DesignerCommonService>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            // 📂 Şəkil yüklə (əgər varsa)
            if (dto.CardImage != null)
            {
                entity.CardImage = await _fileService.UploadFile(dto.CardImage, "designer-common-services");
            }
            else
            {
                entity.CardImage = string.Empty;
            }

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<DesignerCommonServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Tək xidmət məlumatı
        // ───────────────────────────────
        public async Task<DesignerCommonServiceDto?> GetDesignerCommonServiceAsync(string designerCommonServiceId)
        {
            if (!Guid.TryParse(designerCommonServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                EnableTraking: false
            );

            if (entity == null)
                throw new GlobalAppException("Xidmət tapılmadı.");

            return _mapper.Map<DesignerCommonServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Bütün xidmətləri gətir
        // ───────────────────────────────
        public async Task<List<DesignerCommonServiceDto>> GetAllCategoriesAsync()
        {
            var list = await _read.GetAllAsync(
                x => !x.IsDeleted,
                EnableTraking: false,
                orderBy: q => q.OrderBy(x => x.CreatedDate)
            );

            return list.Select(_mapper.Map<DesignerCommonServiceDto>).ToList();
        }

        // ───────────────────────────────
        // ✅ Xidməti yenilə
        // ───────────────────────────────
        public async Task<DesignerCommonServiceDto> UpdateDesignerCommonServiceAsync(UpdateDesignerCommonServiceDto dto)
        {
            if (dto == null || !Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // 🔤 Text sahələri yenilə
            _mapper.Map(dto, entity);

            // 📂 Şəkil yenilənibsə
            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _fileService.DeleteFile("designer-common-services", entity.CardImage);

                entity.CardImage = await _fileService.UploadFile(dto.CardImage, "designer-common-services");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<DesignerCommonServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Xidməti sil (soft delete)
        // ───────────────────────────────
        public async Task DeleteDesignerCommonServiceAsync(string designerCommonServiceId)
        {
            if (!Guid.TryParse(designerCommonServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == id && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // 📂 Əgər şəkil varsa sil
            if (!string.IsNullOrWhiteSpace(entity.CardImage))
                await _fileService.DeleteFile("designer-common-services", entity.CardImage);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
