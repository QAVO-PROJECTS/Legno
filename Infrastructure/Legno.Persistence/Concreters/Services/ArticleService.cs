using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Article;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Legno.Infrastructure.Concreters.Services;
using Microsoft.EntityFrameworkCore;

namespace Legno.Persistence.Concreters.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleReadRepository _read;
        private readonly IArticleWriteRepository _write;
        private readonly IArticleImageWriteRepository _imgWrite;
        private readonly IArticleImageReadRepository _imgRead;
        private readonly IFileService _file;
        private readonly IMapper _mapper;

        public ArticleService(
            IArticleReadRepository read,
            IArticleWriteRepository write,
            IArticleImageWriteRepository imgWrite,
            IArticleImageReadRepository imgRead,
            IFileService file,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _imgWrite = imgWrite;
            _imgRead = imgRead;
            _file = file;
            _mapper = mapper;
        }

        // ---------------- CREATE ----------------
        public async Task<ArticleDto> AddArticleAsync(CreateArticleDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Article>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.AddAsync(entity);
            if (dto.CardImage != null)
            {
                entity.CardImage = await _file.UploadFile(dto.CardImage, "articles");
            }

            if (dto.AuthorImage != null)
            {
                entity.AuthorImage = await _file.UploadFile(dto.AuthorImage, "articles/author");
            }

            if (dto.Images?.Any() == true)
            {
                foreach (var file in dto.Images)
                {
                    var name = await _file.UploadFile(file, "articles/gallery");

                    await _imgWrite.AddAsync(new ArticleImage
                    {
                        Id = Guid.NewGuid(),
                        ArticleId = entity.Id,
                        Name = name,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }

            await _write.CommitAsync();

            return _mapper.Map<ArticleDto>(entity);
        }

        // ---------------- GET BY ID ----------------
        public async Task<ArticleDto?> GetArticleAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == gid && !x.IsDeleted,
                include: q => q.Include(i => i.Images.Where(x => !x.IsDeleted)),
                EnableTraking: false
            );

            if (entity == null)
                throw new GlobalAppException("Məqalə tapılmadı.");

            return _mapper.Map<ArticleDto>(entity);
        }

        // ---------------- GET ALL ----------------
        public async Task<List<ArticleDto>> GetAllArticlesAsync()
        {
            var list = await _read.GetAllAsync(
                func: x => !x.IsDeleted,
                include: q => q.Include(i => i.Images.Where(x => !x.IsDeleted)),
                orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                EnableTraking: false
            );

            return _mapper.Map<List<ArticleDto>>(list);
        }

        // ---------------- UPDATE ----------------
        public async Task<ArticleDto> UpdateArticleAsync(UpdateArticleDto dto)
        {
            if (dto == null || !Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _read.GetAsync(
                a => a.Id == id && !a.IsDeleted,
                include: q => q.Include(a => a.Images),
                EnableTraking: true
            ) ?? throw new GlobalAppException("Məqalə tapılmadı.");



            // ================= DELETE GALLERY IMAGES =================
            if (dto.DeleteImageIds?.Any() == true)
            {
                var deleteSet = new HashSet<string>(dto.DeleteImageIds, StringComparer.OrdinalIgnoreCase);

                var toDelete = entity.Images
                    .Where(x => !x.IsDeleted && deleteSet.Contains(x.Name))
                    .ToList();

                foreach (var img in toDelete)
                {
                    await _file.DeleteFile("articles/gallery", img.Name);

                    img.IsDeleted = true;
                    img.DeletedDate = DateTime.UtcNow;
                    img.LastUpdatedDate = DateTime.UtcNow;
                }
            }



            // ================= CARD IMAGE =================
            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _file.DeleteFile("articles", entity.CardImage);

                entity.CardImage = await _file.UploadFile(dto.CardImage, "articles");
            }



            // ================= AUTHOR IMAGE =================
            if (dto.AuthorImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.AuthorImage))
                    await _file.DeleteFile("articles/author", entity.AuthorImage);

                entity.AuthorImage = await _file.UploadFile(dto.AuthorImage, "articles/author");
            }



            // ================= TEXT FIELDS =================
            entity.Title = dto.Title ?? entity.Title;
            entity.TitleEng = dto.TitleEng ?? entity.TitleEng;
            entity.TitleRu = dto.TitleRu ?? entity.TitleRu;

            entity.SubTitle = dto.SubTitle ?? entity.SubTitle;
            entity.SubTitleEng = dto.SubTitleEng ?? entity.SubTitleEng;
            entity.SubTitleRu = dto.SubTitleRu ?? entity.SubTitleRu;

            entity.AuthorName = dto.AuthorName ?? entity.AuthorName;
            entity.AuthorNameEng = dto.AuthorNameEng ?? entity.AuthorNameEng;
            entity.AuthorNameRu = dto.AuthorNameRu ?? entity.AuthorNameRu;

            entity.LastUpdatedDate = DateTime.UtcNow;



            // ================= ADD NEW IMAGES =================
            if (dto.Images?.Any() == true)
            {
                foreach (var file in dto.Images)
                {
                    var name = await _file.UploadFile(file, "articles/gallery");

                    await _imgWrite.AddAsync(new ArticleImage
                    {
                        Id = Guid.NewGuid(),
                        ArticleId = entity.Id,
                        Name = name,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }



            // ================= SAVE =================
            await _write.UpdateAsync(entity);

            await _write.CommitAsync();

            return _mapper.Map<ArticleDto>(entity);
        }




        // ---------------- DELETE ----------------
        public async Task DeleteArticleAsync(string id)
        {
            if (!Guid.TryParse(id, out var gid))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetAsync(
                x => x.Id == gid && !x.IsDeleted,
                include: q => q.Include(i => i.Images),
                EnableTraking: true
            ) ?? throw new GlobalAppException("Məqalə tapılmadı.");

            // şəkilləri sil
            if (entity.Images?.Any() == true)
            {
                foreach (var img in entity.Images)
                {
                    await _file.DeleteFile("articles/gallery", img.Name);
                    img.IsDeleted = true;
                    img.DeletedDate = DateTime.UtcNow;
                }
            }

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
