using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.BusinessService;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class BusinessServiceService : IBusinessServiceService
    {
        private readonly IBusinessServiceReadRepository _read;
        private readonly IBusinessServiceWriteRepository _write;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public BusinessServiceService(
            IBusinessServiceReadRepository read,
            IBusinessServiceWriteRepository write,
            IFileService fileService,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _fileService = fileService;
            _mapper = mapper;
        }

        // ───────────────────────────────────────────────
        // ✅ Yeni Xidmət Əlavə Et
        // ───────────────────────────────────────────────
        public async Task<BusinessServiceDto> AddBusinessServiceAsync(CreateBusinessServiceDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            if (dto.CardImage == null)
                throw new GlobalAppException("Kart şəkli tələb olunur.");

            var entity = _mapper.Map<BusinessService>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            // 📂 Şəkli FileService vasitəsilə yüklə
            entity.CardImage = await _fileService.UploadFile(dto.CardImage, "businessservices");

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────────────────────
        // ✅ Tək Xidməti Getir
        // ───────────────────────────────────────────────
        public async Task<BusinessServiceDto?> GetBusinessServiceAsync(string businessServiceId)
        {
            if (!Guid.TryParse(businessServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: false);
            if (entity == null)
                throw new GlobalAppException("Xidmət tapılmadı.");

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────────────────────
        // ✅ Bütün Xidmətləri Getir
        // ───────────────────────────────────────────────
        public async Task<List<BusinessServiceDto>> GetAllCategoriesAsync()
        {
            var list = await _read.GetAllAsync(
                x => !x.IsDeleted,
                EnableTraking: false,
                orderBy: q => q.OrderBy(x => x.CreatedDate)
            );

            return list.Select(_mapper.Map<BusinessServiceDto>).ToList();
        }

        // ───────────────────────────────────────────────
        // ✅ Xidməti Yenilə
        // ───────────────────────────────────────────────
        public async Task<BusinessServiceDto> UpdateBusinessServiceAsync(UpdateBusinessServiceDto dto)
        {
            if (dto == null || !Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // 🔤 Scalar dəyərlər (null gəlməyənləri yenilə)
            _mapper.Map(dto, entity);

            // 📂 Şəkil dəyişibsə yenilə
            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _fileService.DeleteFile("businessservices", entity.CardImage);

                entity.CardImage = await _fileService.UploadFile(dto.CardImage, "businessservices");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        // ───────────────────────────────────────────────
        // ✅ Xidməti Sil (Soft Delete)
        // ───────────────────────────────────────────────
        public async Task DeleteBusinessServiceAsync(string businessServiceId)
        {
            if (!Guid.TryParse(businessServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // 📂 Əgər şəkil varsa sil
            if (!string.IsNullOrEmpty(entity.CardImage))
                await _fileService.DeleteFile("businessservices", entity.CardImage);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
