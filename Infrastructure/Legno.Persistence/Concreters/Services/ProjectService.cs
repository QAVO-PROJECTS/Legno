using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories.ProjectImages;
using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Application.Abstracts.Repositories.ProjectVideos;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Project;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Legno.Persistence.Concreters.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectReadRepository _projectRead;
        private readonly IProjectWriteRepository _projectWrite;
        private readonly IProjectImageReadRepository _imageRead;
        private readonly IProjectImageWriteRepository _imageWrite;
        private readonly IProjectVideoReadRepository _videoRead;
        private readonly IProjectVideoWriteRepository _videoWrite;
        private readonly IFileService _fileService;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public ProjectService(
            IProjectReadRepository projectRead,
            IProjectWriteRepository projectWrite,
            IProjectImageReadRepository imageRead,
            IProjectImageWriteRepository imageWrite,
            IProjectVideoReadRepository videoRead,
            IProjectVideoWriteRepository videoWrite,
            IFileService fileService,
            IMapper mapper,
            CloudinaryService cloudinaryService)
        {
            _projectRead = projectRead;
            _projectWrite = projectWrite;
            _imageRead = imageRead;
            _imageWrite = imageWrite;
            _videoRead = videoRead;
            _videoWrite = videoWrite;
            _fileService = fileService;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ProjectDto> AddProjectAsync(CreateProjectDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");
            if (!Guid.TryParse(dto.CategoryId, out var _))
                throw new GlobalAppException("Category ID formatı yanlışdır.");

            // DisplayOrderId = max + 1
            var all = await _projectRead.GetAllAsync(x => !x.IsDeleted);
            var maxDisplay = all.Any() ? all.Max(x => x.DisplayOrderId) : 0;

            var entity = _mapper.Map<Project>(dto);
            entity.Id = Guid.NewGuid();
            entity.DisplayOrderId = maxDisplay + 1;
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;

            // 🔹 CARD IMAGE (IFormFile)
            if (dto.CardImage != null)
            {
                var storedCard = await _cloudinaryService.UploadFileAsync(dto.CardImage);
                entity.CardImage = storedCard;
            }
            else
            {
                // istəyinə görə “CardImage tələbidir” deyib exception ata bilərsən
                entity.CardImage = entity.CardImage ?? string.Empty;
            }

            await _projectWrite.AddAsync(entity);

            // 🔹 Şəkillər
            if (dto.ProjectImages != null && dto.ProjectImages.Any())
            {
                foreach (var file in dto.ProjectImages)
                {
                    var stored = await _cloudinaryService.UploadFileAsync(file);
                    await _imageWrite.AddAsync(new ProjectImage
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        Name = stored,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            // 🔹 Videolar
            if (dto.ProjectVideos != null && dto.ProjectVideos.Any())
            {
                foreach (var file in dto.ProjectVideos)
                {
                    var stored = await _cloudinaryService.UploadFileAsync(file);
                    await _videoWrite.AddAsync(new ProjectVideo
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        Name = stored,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _projectWrite.CommitAsync();
            await _imageWrite.CommitAsync();
            await _videoWrite.CommitAsync();

            var created = await _projectRead.GetAsync(
                p => p.Id == entity.Id && !p.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                EnableTraking: false);

            return _mapper.Map<ProjectDto>(created);
        }
        public async Task<List<ProjectDto>> GetProjectsByCategoryAsync(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                throw new GlobalAppException("Category Id tələb olunur.");

            if (!Guid.TryParse(categoryId, out var catId))
                throw new GlobalAppException("Category ID formatı yanlışdır.");

            var list = await _projectRead.GetAllAsync(
                func: p => !p.IsDeleted && p.CategoryId == catId && !p.Category.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                orderBy: q => q.OrderBy(p => p.DisplayOrderId).ThenBy(p => p.CreatedDate),
                EnableTraking: false
            );

            return _mapper.Map<List<ProjectDto>>(list);
        }

        public async Task<ProjectDto?> GetProjectAsync(string projectId)
        {
            if (!Guid.TryParse(projectId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var project = await _projectRead.GetAsync(
                p => p.Id == id && !p.IsDeleted && !p.Category.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                EnableTraking: false);

            if (project == null) throw new GlobalAppException("Layihə tapılmadı.");
            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            var list = await _projectRead.GetAllAsync(
                func: x => !x.IsDeleted && !x.Category.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                orderBy: q => q.OrderBy(p => p.DisplayOrderId).ThenBy(p => p.CreatedDate),
                EnableTraking: false
            );
            return _mapper.Map<List<ProjectDto>>(list);
        }

        public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
                throw new GlobalAppException("Id tələb olunur.");
            if (!Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _projectRead.GetAsync(
                p => p.Id == id && !p.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                EnableTraking: true);

            if (entity == null) throw new GlobalAppException("Layihə tapılmadı.");

            // 1) SİLİNƏCƏK İMAGE/VİDEO-LAR
            if (dto.DeleteImageNames != null && dto.DeleteImageNames.Count > 0 && entity.ProjectImages != null)
            {
                var delImgSet = new HashSet<string>(dto.DeleteImageNames.Where(n => !string.IsNullOrWhiteSpace(n)),
                                                    StringComparer.OrdinalIgnoreCase);

                var toDeleteImgs = entity.ProjectImages
                    .Where(i => !i.IsDeleted && delImgSet.Contains(i.Name))
                    .ToList();

                foreach (var img in toDeleteImgs)
                {
                    await _cloudinaryService.DeleteFileAsync( img.Name);
                    img.IsDeleted = true;
                    img.DeletedDate = DateTime.UtcNow;
                    img.LastUpdatedDate = DateTime.UtcNow;
                    await _imageWrite.UpdateAsync(img);
                }
            }

            if (dto.DeleteVideoNames != null && dto.DeleteVideoNames.Count > 0 && entity.ProjectVideos != null)
            {
                var delVidSet = new HashSet<string>(dto.DeleteVideoNames.Where(n => !string.IsNullOrWhiteSpace(n)),
                                                    StringComparer.OrdinalIgnoreCase);

                var toDeleteVids = entity.ProjectVideos
                    .Where(v => !v.IsDeleted && delVidSet.Contains(v.Name))
                    .ToList();

                foreach (var vid in toDeleteVids)
                {
                    await _cloudinaryService.DeleteFileAsync( vid.Name);
                    vid.IsDeleted = true;
                    vid.DeletedDate = DateTime.UtcNow;
                    vid.LastUpdatedDate = DateTime.UtcNow;
                    await _videoWrite.UpdateAsync(vid);
                }
            }

            // 2) SCALAR FIELDS — yalnız göndərilənləri yaz
            if (!string.IsNullOrWhiteSpace(dto.Title)) entity.Title = dto.Title;
            if (!string.IsNullOrWhiteSpace(dto.TitleEng)) entity.TitleEng = dto.TitleEng;
            if (!string.IsNullOrWhiteSpace(dto.TitleRu)) entity.TitleRu = dto.TitleRu;
            if (!string.IsNullOrWhiteSpace(dto.AuthorName)) entity.AuthorName = dto.AuthorName;


            if (dto.SubTitle != null) entity.SubTitle = dto.SubTitle;
            if (dto.SubTitleEng != null) entity.SubTitleEng = dto.SubTitleEng;
            if (dto.SubTitleRu != null) entity.SubTitleRu = dto.SubTitleRu;

            // 🔹 CARD IMAGE (IFormFile) — köhnəni sil, yenisini yaz
            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _cloudinaryService.DeleteFileAsync( entity.CardImage);

                var storedCard = await _cloudinaryService.UploadFileAsync(dto.CardImage);
                entity.CardImage = storedCard;
            }

            if (!string.IsNullOrWhiteSpace(dto.CategoryId))
            {
                if (!Guid.TryParse(dto.CategoryId, out var newCatId))
                    throw new GlobalAppException("Category ID formatı yanlışdır.");
                entity.CategoryId = newCatId;
            }

            // Update zamanı da DisplayOrderId = max + 1 (team məntiqi)
            var others = await _projectRead.GetAllAsync(x => !x.IsDeleted && x.Id != id);
            var maxDisplay = others.Any() ? others.Max(x => x.DisplayOrderId) : 0;
            entity.DisplayOrderId = maxDisplay + 1;

            entity.LastUpdatedDate = DateTime.UtcNow;

            // 3) YENİ FAYLLAR
            if (dto.ProjectImages != null && dto.ProjectImages.Any())
            {
                foreach (var f in dto.ProjectImages)
                {
                    var stored = await _cloudinaryService.UploadFileAsync(f);
                    await _imageWrite.AddAsync(new ProjectImage
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        Name = stored,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            if (dto.ProjectVideos != null && dto.ProjectVideos.Any())
            {
                foreach (var f in dto.ProjectVideos)
                {
                    var stored = await _cloudinaryService.UploadFileAsync(f);
                    await _videoWrite.AddAsync(new ProjectVideo
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        Name = stored,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _projectWrite.UpdateAsync(entity);
            await _imageWrite.CommitAsync();
            await _videoWrite.CommitAsync();
            await _projectWrite.CommitAsync();

            return _mapper.Map<ProjectDto>(entity);
        }

        public async Task DeleteProjectAsync(string projectId)
        {
            if (!Guid.TryParse(projectId, out var _))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _projectRead.GetByIdAsync(projectId, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Layihə tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _projectWrite.UpdateAsync(entity);
            await _projectWrite.CommitAsync();
        }

        public async Task BulkReorderAsync(List<ProjectReorderDto> items)
        {
            if (items == null || items.Count == 0) return;

            var idSet = new HashSet<Guid>();
            foreach (var it in items)
                if (Guid.TryParse(it.Id, out var gid)) idSet.Add(gid);

            var projects = await _projectRead.GetAllAsync(x => idSet.Contains(x.Id) && !x.IsDeleted);
            foreach (var p in projects)
            {
                var dto = items.FirstOrDefault(i => Guid.TryParse(i.Id, out var gid) && gid == p.Id);
                if (dto != null)
                {
                    p.DisplayOrderId = dto.DisplayOrderId;
                    p.LastUpdatedDate = DateTime.UtcNow;
                    await _projectWrite.UpdateAsync(p);
                }
            }
            await _projectWrite.CommitAsync();
        }
    }
}
