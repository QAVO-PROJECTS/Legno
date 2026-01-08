using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Repositories.Categories;
using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Category;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Legno.Persistence.Concreters.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryReadRepository _read;
        private readonly ICategoryWriteRepository _write;
        private readonly IProjectReadRepository _projectRead;
        private readonly IProjectWriteRepository _projectWrite;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly ICategoryImageReadRepository _categoryImageRead;
        private readonly ICategoryImageWriteRepository _categoryImageWrite;

        public CategoryService(
            ICategoryReadRepository read,
            ICategoryWriteRepository write,
            IMapper mapper,
            IProjectReadRepository projectRead,
            IProjectWriteRepository projectWrite,
            IFileService fileService,
            ICategoryImageReadRepository categoryImageRead,
            ICategoryImageWriteRepository categoryImageWrite)
        {
            _read = read;
            _write = write;
            _mapper = mapper;
            _projectRead = projectRead;
            _projectWrite = projectWrite;
            _fileService = fileService;
            _categoryImageRead = categoryImageRead;
            _categoryImageWrite = categoryImageWrite;
        }

        // ───────────────────────────────
        // ✅ Yeni Kateqoriya Əlavə Et
        // ───────────────────────────────
        public async Task<CategoryDto> AddCategoryAsync(CreateCategoryDto dto)
        {
            if (dto == null)
                throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Category>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            // 📂 Şəkil yüklə (əgər varsa)
            if (dto.CategoryImage != null)
            {
                entity.CategoryImage = await _fileService.UploadFile(dto.CategoryImage, "categories");
            }
            else
            {
                // Şəkil vacibdirsə, burada exception ata bilərsən
                entity.CategoryImage = string.Empty;
            }
            if (dto.CategorySliderImages?.Any() == true)
            {
                foreach (var file in dto.CategorySliderImages)
                {
                    var fileName = await _fileService.UploadFile(file, "category/images");
                    await _categoryImageWrite.AddAsync(new CategoryImage
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = entity.Id,
                        Name = fileName,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }
            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<CategoryDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Tək Kateqoriyanı Getir
        // ───────────────────────────────
        public async Task<CategoryDto?> GetCategoryAsync(string categoryId)
        {
            if (!Guid.TryParse(categoryId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var category = await _read.GetAsync(
                c => c.Id == id && !c.IsDeleted,
               include: q => q

                    .Include(p => p.CategorySliderImages.Where(i => !i.IsDeleted)),
                EnableTraking: false
            );

            if (category == null)
                throw new GlobalAppException("Kateqoriya tapılmadı.");

            return _mapper.Map<CategoryDto>(category);
        }

        // ───────────────────────────────
        // ✅ Bütün Kateqoriyaları Getir
        // ───────────────────────────────
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _read.GetAllAsync(
                func: c => !c.IsDeleted,
                  include: q => q
                    .Include(p => p.CategorySliderImages.Where(i => !i.IsDeleted)),
                orderBy: q => q.OrderBy(c => c.CreatedDate),
                EnableTraking: false
            );

            return _mapper.Map<List<CategoryDto>>(categories);
        }

        // ───────────────────────────────
        // ✅ Kateqoriya Yenilə
        // ───────────────────────────────
        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _read.GetAsync(
                p => p.Id.ToString() == dto.Id && !p.IsDeleted,
                include: q => q

                 
                    .Include(p => p.CategorySliderImages.Where(i => !i.IsDeleted)),
               

                EnableTraking: true) ;
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Kateqoriya tapılmadı.");

            // 📂 Əgər yeni şəkil yüklənibsə
            if (dto.CategoryImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CategoryImage))
                    await _fileService.DeleteFile("categories", entity.CategoryImage);

                entity.CategoryImage = await _fileService.UploadFile(dto.CategoryImage, "categories");
            }
            if (dto.DeletedCategorySliderImage?.Any() == true && entity.CategorySliderImages != null)
            {
                var set = new HashSet<string>(dto.DeletedCategorySliderImage, StringComparer.OrdinalIgnoreCase);
                var toDelete = entity.CategorySliderImages.Where(i => !i.IsDeleted && set.Contains(i.Name)).ToList();
                foreach (var img in toDelete)
                {
                    await _fileService.DeleteFile("category/images", img.Name);
                    img.IsDeleted = true;
                    img.DeletedDate = DateTime.UtcNow;
                    img.LastUpdatedDate = DateTime.UtcNow;
                    await _categoryImageWrite.UpdateAsync(img);
                }
            }
            if (dto.CategorySliderImages?.Any() == true)
            {
                foreach (var file in dto.CategorySliderImages)
                {
                    var fileName = await _fileService.UploadFile(file, "category/images");
                    await _categoryImageWrite.AddAsync(new CategoryImage
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = entity.Id,
                        Name = fileName,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }

            // 🔤 Digər dəyərləri yenilə (yalnız null olmayanları)
            if (!string.IsNullOrWhiteSpace(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.NameEng))
                entity.NameEng = dto.NameEng;

            if (!string.IsNullOrWhiteSpace(dto.NameRu))
                entity.NameRu = dto.NameRu;

            entity.LastUpdatedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<CategoryDto>(entity);
        }

        // ───────────────────────────────
        // ✅ Kateqoriyanı Sil (Soft Delete)
        // ───────────────────────────────
        public async Task DeleteCategoryAsync(string categoryId)
        {
            if (!Guid.TryParse(categoryId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetByIdAsync(categoryId, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Kateqoriya tapılmadı.");

            // 📂 Əgər şəkil varsa sil
            if (!string.IsNullOrEmpty(entity.CategoryImage))
                await _fileService.DeleteFile("categories", entity.CategoryImage);

            // Kateqoriyanı soft delete et
            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);

        

            // Hər iki repository üçün commit
            await _projectWrite.CommitAsync();
            await _write.CommitAsync();
        }
    }
}
