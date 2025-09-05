using AutoMapper;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.BusinessService;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Application.Abstracts.Repositories; // IBusinessServiceReadRepository, IBusinessServiceWriteRepository

namespace Legno.Persistence.Concreters.Services
{
    public class BusinessServiceService : IBusinessServiceService
    {
        private readonly IBusinessServiceReadRepository _read;
        private readonly IBusinessServiceWriteRepository _write;
        private readonly CloudinaryService _cloudinary; // şəkil yükləmə
        private readonly IMapper _mapper;

        public BusinessServiceService(
            IBusinessServiceReadRepository read,
            IBusinessServiceWriteRepository write,
            CloudinaryService cloudinary,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _cloudinary = cloudinary;
            _mapper = mapper;
        }

        public async Task<BusinessServiceDto> AddBusinessServiceAsync(CreateBusinessServiceDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib."); // :contentReference[oaicite:3]{index=3}

            var entity = _mapper.Map<BusinessService>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            // CardImage faylı tələb olunur (DTO-da var) :contentReference[oaicite:4]{index=4}
            if (dto.CardImage == null) throw new GlobalAppException("Kart şəkli tələb olunur.");
            entity.CardImage = await _cloudinary.UploadFileAsync(dto.CardImage);

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity); // :contentReference[oaicite:5]{index=5}
        }

        public async Task<BusinessServiceDto?> GetBusinessServiceAsync(string BusinessServiceId)
        {
            if (!Guid.TryParse(BusinessServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: false);
            if (entity == null) throw new GlobalAppException("Xidmət tapılmadı.");

            return _mapper.Map<BusinessServiceDto>(entity); // :contentReference[oaicite:6]{index=6}
        }

        // Adı interfeysdə belə verilib — hamısını qaytarır
        public async Task<List<BusinessServiceDto>> GetAllCategoriesAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted,
                                               EnableTraking: false,
                                               orderBy: q => q.OrderBy(x => x.CreatedDate));
            return list.Select(_mapper.Map<BusinessServiceDto>).ToList(); // :contentReference[oaicite:7]{index=7}
        }

        public async Task<BusinessServiceDto> UpdateBusinessServiceAsync(UpdateBusinessServiceDto dto)
        {
            if (dto == null || !Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID."); // :contentReference[oaicite:8]{index=8}

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // Ad, AdEng, AdRu kimi scalar-lar (null-lar toxunulmur) :contentReference[oaicite:9]{index=9}
            _mapper.Map(dto, entity);

            // Kart şəkli göndərilibsə yenilə (DTO-da CardImage mövcuddur) :contentReference[oaicite:10]{index=10}
            if (dto.CardImage != null)
            {
                // köhnəni silmək istəyirsinizsə:
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _cloudinary.DeleteFileAsync(entity.CardImage);

                entity.CardImage = await _cloudinary.UploadFileAsync(dto.CardImage);
            }

            entity.LastUpdatedDate = DateTime.UtcNow;
            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity); // :contentReference[oaicite:11]{index=11}
        }

        public async Task DeleteBusinessServiceAsync(string BusinessServiceId)
        {
            if (!Guid.TryParse(BusinessServiceId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
