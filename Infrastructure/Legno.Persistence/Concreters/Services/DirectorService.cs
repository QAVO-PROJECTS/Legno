using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;

using Legno.Application.Dtos.Director;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class DirectorService : IDirectorService
    {
        private readonly IDirectorReadRepository _read; 
        private readonly IDirectorWriteRepository _write;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public DirectorService(
            IDirectorReadRepository read,
            IDirectorWriteRepository write,
            IFileService fileService,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<DirectorDto> AddDirectorAsync(CreateDirectorDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Director>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            if (dto.Image != null)
                entity.Image = await _fileService.UploadFile(dto.Image, "directors");

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<DirectorDto>(entity);
        }

        public async Task<DirectorDto?> GetDirectorAsync(string id)
        {
            var entity = await _read.GetByIdAsync(id, EnableTraking: false);

            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Director tapılmadı.");

            return _mapper.Map<DirectorDto>(entity);
        }

        public async Task<List<DirectorDto>> GetAllDirectorsAsync()
        {
            var list = await _read.GetAllAsync(
                d => !d.IsDeleted,
                EnableTraking: false);

            return _mapper.Map<List<DirectorDto>>(list);
        }

        public async Task<DirectorDto> UpdateDirectorAsync(UpdateDirectorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _read.GetByIdAsync(dto.Id, EnableTraking: true)
                ?? throw new GlobalAppException("Director tapılmadı.");

            _mapper.Map(dto, entity);

            if (dto.Image != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.Image))
                    await _fileService.DeleteFile("directors", entity.Image);

                entity.Image = await _fileService.UploadFile(dto.Image, "directors");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<DirectorDto>(entity);
        }

        public async Task DeleteDirectorAsync(string id)
        {
            var entity = await _read.GetByIdAsync(id, EnableTraking: true)
                ?? throw new GlobalAppException("Director tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
