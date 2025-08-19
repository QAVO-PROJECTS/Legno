using AutoMapper;
using Legno.Application.Abstracts.Repositories;
using Legno.Application.Abstracts.Repositories.Categories;
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

        public CategoryService(
          ICategoryReadRepository read,
        ICategoryWriteRepository write,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _mapper = mapper;
        }

        public async Task<CategoryDto> AddCategoryAsync(CreateCategoryDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");

            var entity = _mapper.Map<Category>(dto);
            entity.Id = Guid.NewGuid();
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

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

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _write.UpdateAsync(entity);
            await _write.CommitAsync();
        }
    }
}
