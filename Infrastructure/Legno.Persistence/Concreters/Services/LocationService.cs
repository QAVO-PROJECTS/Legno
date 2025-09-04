using AutoMapper;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Location;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Application.Abstracts.Repositories; // ILocationReadRepository, ILocationWriteRepository

namespace Legno.Persistence.Concreters.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationReadRepository _read;
        private readonly ILocationWriteRepository _write;
        private readonly IMapper _mapper;

        public LocationService(ILocationReadRepository read, ILocationWriteRepository write, IMapper mapper)
        {
            _read = read;
            _write = write;
            _mapper = mapper;
        }

        public async Task<LocationDto> AddLocationAsync(CreateLocationDto createLocationDto)
        {
            if (createLocationDto == null) throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Location>(createLocationDto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<LocationDto>(entity);
        }

        public async Task<LocationDto?> GetLocationAsync(string locationId)
        {
            if (!Guid.TryParse(locationId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: false);
            if (entity == null) throw new GlobalAppException("Məkan tapılmadı.");

            return _mapper.Map<LocationDto>(entity);
        }

        public async Task<List<LocationDto>> GetAllLocationsAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted, EnableTraking: false,
                orderBy: q => q.OrderBy(x => x.CreatedDate));
            return list.Select(_mapper.Map<LocationDto>).ToList();
        }

        public async Task<LocationDto> UpdateLocationAsync(UpdateLocationDto updateLocationDto)
        {
            if (updateLocationDto == null || !Guid.TryParse(updateLocationDto.Id, out var id))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Məkan tapılmadı.");

            // Null gəlməyən sahələri tətbiq edirik (Profile-də null-ignore artıq var)
            _mapper.Map(updateLocationDto, entity);
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<LocationDto>(entity);
        }

        public async Task DeleteLocationAsync(string locationId)
        {
            if (!Guid.TryParse(locationId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == id && !x.IsDeleted, EnableTraking: true)
                        ?? throw new GlobalAppException("Məkan tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
