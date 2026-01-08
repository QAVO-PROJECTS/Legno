using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.BusinessService;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerReadRepository _read;
        private readonly IPartnerWriteRepository _write;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService; // 🔁 Cloudinary əvəzinə

        public PartnerService(
            IPartnerReadRepository read,
            IPartnerWriteRepository write,
            IMapper mapper,
            IFileService fileService)
        {
            _read = read;
            _write = write;
            _mapper = mapper;
            _fileService = fileService;
        }

        // ───────────────────────────────
        // ✅ Yeni partnyor əlavə et
        // ───────────────────────────────
        public async Task<BusinessServiceDto> AddBusinessServiceAsync(CreateBusinessServiceDto createDto)
        {
            if (createDto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Partner>(createDto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            // 📂 Şəkil faylı tələb olunur
            if (createDto.CardImage == null)
                throw new GlobalAppException("Kart şəkli tələb olunur.");

            // Faylı serverə yüklə
            var storedFileName = await _fileService.UploadFile(createDto.CardImage, "partners");
            entity.CardImage = storedFileName;

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Tək partnyoru gətir
        // ───────────────────────────────
        public async Task<BusinessServiceDto?> GetBusinessServiceAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == gid && !x.IsDeleted,
                EnableTraking: false
            ) ?? throw new GlobalAppException("Partnyor tapılmadı.");

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Bütün partnyorları gətir
        // ───────────────────────────────
        public async Task<List<BusinessServiceDto>> GetAllBusinessServicesAsync()
        {
            var list = await _read.GetAllAsync(
                x => !x.IsDeleted,
                EnableTraking: false,
                orderBy: q => q.OrderBy(x => x.CreatedDate)
            );

            return list.Select(_mapper.Map<BusinessServiceDto>).ToList();
        }

        // ───────────────────────────────
        // ✅ Partnyor məlumatını yenilə
        // ───────────────────────────────
        public async Task<BusinessServiceDto> UpdateBusinessServiceAsync(UpdateBusinessServiceDto updateDto)
        {
            if (updateDto == null || !Guid.TryParse(updateDto.Id, out var gid))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(
                x => x.Id == gid && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Partnyor tapılmadı.");

            _mapper.Map(updateDto, entity);
            entity.LastUpdatedDate = DateTime.UtcNow;

            // 📂 Yeni şəkil yüklənibsə, köhnəni sil və yenisini saxla
            if (updateDto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _fileService.DeleteFile("partners", entity.CardImage);

                var storedFileName = await _fileService.UploadFile(updateDto.CardImage, "partners");
                entity.CardImage = storedFileName;
            }

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Partnyoru sil (soft delete + şəkil sil)
        // ───────────────────────────────
        public async Task DeleteBusinessServiceAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == gid && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Partnyor tapılmadı.");

            // 📂 Əgər şəkil varsa — sil
            if (!string.IsNullOrWhiteSpace(entity.CardImage))
                await _fileService.DeleteFile("partners", entity.CardImage);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
