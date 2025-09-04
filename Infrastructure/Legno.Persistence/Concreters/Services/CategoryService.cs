using AutoMapper;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Repositories.Categories;
using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Category;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryReadRepository _read;
        private readonly ICategoryWriteRepository _write;
        private readonly IMapper _mapper;
        private readonly IProjectReadRepository _projectRead;
        private readonly IProjectWriteRepository _projectWrite;
        private readonly CloudinaryService _cloudinaryService;

        public CategoryService(
          ICategoryReadRepository read,
        ICategoryWriteRepository write,
            IMapper mapper,
            IProjectReadRepository projectRead,
            IProjectWriteRepository projectWrite,
            CloudinaryService cloudinaryService)
        {
            _read = read;
            _write = write;
            _mapper = mapper;
            _projectRead = projectRead;
            _projectWrite = projectWrite;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<CategoryDto> AddCategoryAsync(CreateCategoryDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Category>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            if (dto.CategoryImage != null)
            {
                var storedCard = await _cloudinaryService.UploadFileAsync(dto.CategoryImage);
                entity.CategoryImage = storedCard;
            }
            else
            {
                // istəyinə görə “CardImage tələbidir” deyib exception ata bilərsən
                entity.CategoryImage = entity.CategoryImage ?? string.Empty;
            }

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<CategoryDto>(entity);
        }

        public async Task<CategoryDto?> GetCategoryAsync(string categoryId)
        {
            if (!Guid.TryParse(categoryId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var cat = await _read.GetAsync(
                c => c.Id == id && !c.IsDeleted,
                include: null,
                EnableTraking: false);

            if (cat == null) throw new GlobalAppException("Kateqoriya tapılmadı.");
            return _mapper.Map<CategoryDto>(cat);
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var cats = await _read.GetAllAsync(
                func: x => !x.IsDeleted,
                include: null,
                orderBy: q => q.OrderBy(x => x.CreatedDate),
                EnableTraking: false
            );
            return _mapper.Map<List<CategoryDto>>(cats);
        }
        public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _read.GetByIdAsync(dto.Id, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Kateqoriya tapılmadı.");
            if (dto.CategoryImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CategoryImage))
                    await _cloudinaryService.DeleteFileAsync(entity.CategoryImage);

                var storedCard = await _cloudinaryService.UploadFileAsync(dto.CategoryImage);
                entity.CategoryImage = storedCard;
            }
            // Manual update: null/boş dəyərlər mövcudu örtmür
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

        public async Task DeleteCategoryAsync(string categoryId)
        {
            if (!Guid.TryParse(categoryId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _read.GetByIdAsync(categoryId, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Kateqoriya tapılmadı.");

            // (İmkan varsa) eyni DbContext üzərindən tranzaksiya açın
            // Repozitorinizdə BeginTransaction yoxdursa, DbContext-ə çıxış verən UoW istifadə edin.
            // Misal üçün:
            // using var tx = await _write.BeginTransactionAsync();

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            await _write.UpdateAsync(entity);

            // Bu kateqoriyadakı bütün layihələri tap və soft delete et
            var projects = await _projectRead.GetAllAsync(
                func: p => p.CategoryId == id && !p.IsDeleted,
                include: null,
                orderBy: null,
                EnableTraking: true
            );

            foreach (var p in projects)
            {
                p.IsDeleted = true;
                p.DeletedDate = DateTime.UtcNow;
                await _projectWrite.UpdateAsync(p);
            }
            await _projectWrite.CommitAsync();
            // Tək commit (əgər yazı repozitoriləri eyni DbContext-i paylaşırsa)
            await _write.CommitAsync();
            // await tx.CommitAsync();
        }
    }
}
