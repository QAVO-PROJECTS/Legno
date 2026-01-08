using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Announcement;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementReadRepository _read;
        private readonly IAnnouncementWriteRepository _write;
        private readonly IFileService _file;
        private readonly IMapper _mapper;

        public AnnouncementService(
            IAnnouncementReadRepository read,
            IAnnouncementWriteRepository write,
            IFileService file,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _file = file;
            _mapper = mapper;
        }

        // ───────────────────────────────
        // ✅ Yeni Elan Əlavə Et
        // ───────────────────────────────
        public async Task<AnnouncementDto> AddAnnouncementAsync(CreateAnnouncementDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Announcement>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            if (dto.AuthorImage != null)
                entity.AuthorImage = await _file.UploadFile(dto.AuthorImage, "announcement/authors");

            if (dto.CardImage != null)
                entity.CardImage = await _file.UploadFile(dto.CardImage, "announcement/cards");

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<AnnouncementDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Tək Elanı Getir
        // ───────────────────────────────
        public async Task<AnnouncementDto?> GetAnnouncementAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var item = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted);

            if (item == null)
                throw new GlobalAppException("Elan tapılmadı.");

            return _mapper.Map<AnnouncementDto>(item);
        }

        // ───────────────────────────────
        // ✅ Bütün Elanları Getir
        // ───────────────────────────────
        public async Task<List<AnnouncementDto>> GetAllAnnouncementsAsync()
        {
            var list = await _read.GetAllAsync(
                func: x => !x.IsDeleted,
                orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                EnableTraking: false
            );

            return _mapper.Map<List<AnnouncementDto>>(list);
        }

        // ───────────────────────────────
        // ✅ Elanı Yenilə
        // ───────────────────────────────
        public async Task<AnnouncementDto> UpdateAnnouncementAsync(UpdateAnnouncementDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = await _read.GetAsync(x => !x.IsDeleted && x.Id.ToString()==dto.Id)
                ?? throw new GlobalAppException("Elan tapılmadı.");

            // Text sahələri
            if (!string.IsNullOrWhiteSpace(dto.Title))
                entity.Title = dto.Title;
            if (!string.IsNullOrWhiteSpace(dto.TitleEng))
                entity.TitleEng = dto.TitleEng;

            if (!string.IsNullOrWhiteSpace(dto.TitleRu))
                entity.TitleRu = dto.TitleRu;
            if (!string.IsNullOrWhiteSpace(dto.SubTitle))
                entity.SubTitle = dto.SubTitle;
            if (!string.IsNullOrWhiteSpace(dto.SubTitleEng))
                entity.SubTitleEng = dto.SubTitleEng; 
            if (!string.IsNullOrWhiteSpace(dto.SubTitleRu))
                entity.SubTitleRu = dto.SubTitleRu;

            if (!string.IsNullOrWhiteSpace(dto.AuthorName))
                entity.AuthorName = dto.AuthorName;
            if (!string.IsNullOrWhiteSpace(dto.AuthorNameRu))
                entity.AuthorNameRu = dto.AuthorNameRu;
            if (!string.IsNullOrWhiteSpace(dto.AuthorNameEng))
                entity.AuthorNameEng = dto.AuthorNameEng;

            // Şəkillər
            if (dto.AuthorImage != null)
            {
                if (!string.IsNullOrEmpty(entity.AuthorImage))
                    await _file.DeleteFile("announcement/authors", entity.AuthorImage);

                entity.AuthorImage = await _file.UploadFile(dto.AuthorImage, "announcement/authors");
            }

            if (dto.CardImage != null)
            {
                if (!string.IsNullOrEmpty(entity.CardImage))
                    await _file.DeleteFile("announcement/cards", entity.CardImage);

                entity.CardImage = await _file.UploadFile(dto.CardImage, "announcement/cards");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<AnnouncementDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Elanı Sil (Soft Delete)
        // ───────────────────────────────
        public async Task DeleteAnnouncementAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(x => x.Id == gid && !x.IsDeleted)
                ?? throw new GlobalAppException("Elan tapılmadı.");

            if (!string.IsNullOrEmpty(entity.AuthorImage))
                await _file.DeleteFile("announcement/authors", entity.AuthorImage);

            if (!string.IsNullOrEmpty(entity.CardImage))
                await _file.DeleteFile("announcement/cards", entity.CardImage);

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
