using AutoMapper;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.BusinessService;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly IPartnerReadRepository _read;
        private readonly IPartnerWriteRepository _write;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinary;
        public PartnerService(
            IPartnerReadRepository read,
            IPartnerWriteRepository write,
            IMapper mapper,
            CloudinaryService cloudinary)
        {
            _read = read;
            _write = write;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<BusinessServiceDto> AddBusinessServiceAsync(CreateBusinessServiceDto createDto)
        {
            if (createDto == null) throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Partner>(createDto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            if (createDto.CardImage == null) throw new GlobalAppException("Kart şəkli tələb olunur.");
            entity.CardImage = await _cloudinary.UploadFileAsync(createDto.CardImage);


            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        public async Task<BusinessServiceDto?> GetBusinessServiceAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted, EnableTraking: false);
            if (entity == null) throw new GlobalAppException("Xidmət tapılmadı.");

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        public async Task<List<BusinessServiceDto>> GetAllBusinessServicesAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted, EnableTraking: false,
                orderBy: q => q.OrderBy(x => x.CreatedDate));
            return list.Select(_mapper.Map<BusinessServiceDto>).ToList();
        }

        public async Task<BusinessServiceDto> UpdateBusinessServiceAsync(UpdateBusinessServiceDto updateDto)
        {
            if (updateDto == null || !Guid.TryParse(updateDto.Id, out var gid))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            // yalnız göndərilən sahələr (Profile-də null-ignore olmalıdır)
            _mapper.Map(updateDto, entity);
            entity.LastUpdatedDate = DateTime.UtcNow;
            // Kart şəkli göndərilibsə yenilə (DTO-da CardImage mövcuddur) :contentReference[oaicite:10]{index=10}
            if (updateDto.CardImage != null)
            {
                // köhnəni silmək istəyirsinizsə:
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _cloudinary.DeleteFileAsync(entity.CardImage);

                entity.CardImage = await _cloudinary.UploadFileAsync(updateDto.CardImage);
            }
          

                await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<BusinessServiceDto>(entity);
        }

        public async Task DeleteBusinessServiceAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Xidmət tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
