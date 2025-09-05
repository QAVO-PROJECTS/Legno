using AutoMapper;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.DesignerCommonService;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Application.Abstracts.Repositories; // IDesignerCommonServiceReadRepository, IDesignerCommonServiceWriteRepository

namespace Legno.Persistence.Concreters.Services
{
    public class DesignerCommonServiceService : IDesignerCommonServiceService
    {
        private readonly IDesignerCommonServiceReadRepository _read;
        private readonly IDesignerCommonServiceWriteRepository _write;
        private readonly CloudinaryService _cloudinary;
        private readonly IMapper _mapper;

        public DesignerCommonServiceService(
            IDesignerCommonServiceReadRepository read,
            IDesignerCommonServiceWriteRepository write,
            CloudinaryService cloudinary,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _cloudinary = cloudinary;
            _mapper = mapper;
        }

        public async Task<DesignerCommonServiceDto> AddDesignerCommonServiceAsync(CreateDesignerCommonServiceDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<DesignerCommonService>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            // CardImage (opsional)
            if (dto.CardImage != null)
                entity.CardImage = await _cloudinary.UploadFileAsync(dto.CardImage);

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<DesignerCommonServiceDto>(entity);
        }

        public async Task<DesignerCommonServiceDto?> GetDesignerCommonServiceAsync(string designerCommonServiceId)
        {
            if (!Guid.TryParse(designerCommonServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: false)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            return _mapper.Map<DesignerCommonServiceDto>(entity);
        }

        // Interfeys adını qoruyuram (hamısını qaytarır)
        public async Task<List<DesignerCommonServiceDto>> GetAllCategoriesAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted, EnableTraking: false,
                                               orderBy: q => q.OrderBy(x => x.CreatedDate));
            return list.Select(_mapper.Map<DesignerCommonServiceDto>).ToList();
        }

        public async Task<DesignerCommonServiceDto> UpdateDesignerCommonServiceAsync(UpdateDesignerCommonServiceDto dto)
        {
            if (dto == null || !Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // Scalar sahələr
            _mapper.Map(dto, entity);

            // Şəkil yenilənirsə: köhnəni silib yenisini yaz
            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _cloudinary.DeleteFileAsync(entity.CardImage);

                entity.CardImage = await _cloudinary.UploadFileAsync(dto.CardImage);
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<DesignerCommonServiceDto>(entity);
        }

        public async Task DeleteDesignerCommonServiceAsync(string designerCommonServiceId)
        {
            if (!Guid.TryParse(designerCommonServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // İstəyə görə: Cloudinary-dən də silə bilərsiniz
            if (!string.IsNullOrWhiteSpace(entity.CardImage))
                await _cloudinary.DeleteFileAsync(entity.CardImage);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
