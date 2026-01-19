using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Setting;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class SettingService : ISettingService
    {
        private readonly ISettingReadRepository _read;
        private readonly ISettingWriteRepository _write;
        private readonly IFileService _file;
        private readonly IMapper _mapper;

        public SettingService(
            ISettingReadRepository read,
            ISettingWriteRepository write,
            IFileService file,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _file = file;
            _mapper = mapper;
        }

        public async Task<SettingDto> AddSettingAsync(CreateSettingDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            if (string.IsNullOrWhiteSpace(dto.Key))
                throw new GlobalAppException("Key boş ola bilməz!");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new GlobalAppException("Name boş ola bilməz!");

            // Eyni key varsa (deleted olmayan)
            var exists = await _read.GetAsync(x => x.Key == dto.Key && !x.IsDeleted);
            if (exists != null)
                throw new GlobalAppException("Bu Key ilə artıq setting mövcuddur!");

            var entity = _mapper.Map<Setting>(dto);

            entity.Id = Guid.NewGuid();
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            // images upload
            if (dto.ImageOne != null && dto.ImageOne.Length > 0)
                entity.ImageOne = await _file.UploadFile(dto.ImageOne, "settings");

            if (dto.ImageTwo != null && dto.ImageTwo.Length > 0)
                entity.ImageTwo = await _file.UploadFile(dto.ImageTwo, "settings");

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<SettingDto>(entity);
        }

        public async Task<SettingDto?> GetSettingAsync(string settingId)
        {
            if (!Guid.TryParse(settingId, out var guid))
                throw new GlobalAppException("Yanlış ID formatı!");

            var entity = await _read.GetAsync(x => x.Id == guid && !x.IsDeleted);
            return entity == null ? null : _mapper.Map<SettingDto>(entity);
        }
        public async Task<SettingDto?> GetSettingForSettingKeyAsync(string settingKey)
        {
  

            var entity = await _read.GetAsync(x => x.Key==settingKey && !x.IsDeleted);
            return entity == null ? null : _mapper.Map<SettingDto>(entity);
        }

  
        public async Task<List<SettingDto>> GetAllSettingsAsync()
        {
            var list = await _read.GetAllAsync(x => !x.IsDeleted, EnableTraking: false);
            return _mapper.Map<List<SettingDto>>(list);
        }

        public async Task<SettingDto> UpdateSettingAsync(UpdateSettingDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            if (!Guid.TryParse(dto.Id, out var guid))
                throw new GlobalAppException("Yanlış ID!");

            var entity = await _read.GetAsync(
                x => x.Id == guid && !x.IsDeleted,
                EnableTraking: true
            ) ?? throw new GlobalAppException("Setting tapılmadı!");

            // Key update + uniqueness
            if (dto.Key != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Key))
                    throw new GlobalAppException("Key boş ola bilməz!");

                var keyExists = await _read.GetAsync(x => x.Key == dto.Key && x.Id != entity.Id && !x.IsDeleted);
                if (keyExists != null)
                    throw new GlobalAppException("Bu Key başqa setting-də istifadə olunur!");

                entity.Key = dto.Key;
            }

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.Value != null) entity.Value = dto.Value;
            if (dto.ValueEng != null) entity.ValueEng = dto.ValueEng;
            if (dto.ValueRu != null) entity.ValueRu = dto.ValueRu;

            // ================= IMAGE ONE UPDATE =================
            if (dto.ImageOne != null && dto.ImageOne.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(entity.ImageOne))
                    await _file.DeleteFile("settings", entity.ImageOne);

                entity.ImageOne = await _file.UploadFile(dto.ImageOne, "settings");
            }

            // ================= IMAGE TWO UPDATE =================
            if (dto.ImageTwo != null && dto.ImageTwo.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(entity.ImageTwo))
                    await _file.DeleteFile("settings", entity.ImageTwo);

                entity.ImageTwo = await _file.UploadFile(dto.ImageTwo, "settings");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<SettingDto>(entity);
        }

        public async Task DeleteSettingAsync(string settingId)
        {
            if (!Guid.TryParse(settingId, out var guid))
                throw new GlobalAppException("Yanlış ID!");

            var entity = await _read.GetAsync(x => x.Id == guid && !x.IsDeleted)
                ?? throw new GlobalAppException("Setting tapılmadı!");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
