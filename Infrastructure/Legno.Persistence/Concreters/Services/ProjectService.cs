using AutoMapper;
using Legno.Application.Abstracts.Repositories.ProjectImages;
using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Application.Abstracts.Repositories.ProjectVideos;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Project;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Legno.Persistence.Concreters.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectReadRepository _projectRead;
        private readonly IProjectWriteRepository _projectWrite;
        private readonly IProjectImageWriteRepository _imageWrite;
        private readonly IProjectVideoWriteRepository _videoWrite;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public ProjectService(
            IProjectReadRepository projectRead,
            IProjectWriteRepository projectWrite,
            IProjectImageWriteRepository imageWrite,
            IProjectVideoWriteRepository videoWrite,
            CloudinaryService cloudinaryService,
            IMapper mapper)
        {
            _projectRead = projectRead;
            _projectWrite = projectWrite;
            _imageWrite = imageWrite;
            _videoWrite = videoWrite;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
        }

        // CREATE
        public async Task<ProjectDto> AddProjectAsync(CreateProjectDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");
            if (string.IsNullOrWhiteSpace(dto.CategoryId) || !Guid.TryParse(dto.CategoryId, out var catId))
                throw new GlobalAppException("Category ID formatı yanlışdır.");

            // DisplayOrderId = max + 1
            var all = await _projectRead.GetAllAsync(x => !x.IsDeleted);
            var maxDisplay = all.Any() ? all.Max(x => x.DisplayOrderId) : 0;

            var entity = new Project
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                TitleEng = dto.TitleEng,
                TitleRu = dto.TitleRu,
                SubTitle = dto.SubTitle,
                SubTitleEng = dto.SubTitleEng,
                SubTitleRu = dto.SubTitleRu,
                CategoryId = catId,
                TeamId = Guid.TryParse(dto.TeamId, out var teamId) ? teamId : (Guid?)null,
                DisplayOrderId = maxDisplay + 1,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            // Card image
            entity.CardImage = dto.CardImage != null
                ? await _cloudinaryService.UploadFileAsync(dto.CardImage)
                : string.Empty;

            await _projectWrite.AddAsync(entity);

            // Images (IFormFile)
            if (dto.ProjectImages?.Any() == true)
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

            // Videos (ProjectVideoInputDto: YoutubeLink və/və ya File)
            if (dto.ProjectVideos?.Any() == true)
            {
                foreach (var v in dto.ProjectVideos)
                {

                    await _videoWrite.AddAsync(new ProjectVideo
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                           // varsa fayl
                        YoutubeLink = v,      // varsa link
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _imageWrite.CommitAsync();
            await _videoWrite.CommitAsync();
            await _projectWrite.CommitAsync();

            var created = await _projectRead.GetAsync(
                p => p.Id == entity.Id && !p.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                EnableTraking: false);

            return _mapper.Map<ProjectDto>(created);
        }

        // READ (by id)
        public async Task<ProjectDto?> GetProjectAsync(string projectId)
        {
            if (!Guid.TryParse(projectId, out var id))
                throw new GlobalAppException("Yanlış ID formatı.");

            var project = await _projectRead.GetAsync(
                p => p.Id == id && !p.IsDeleted && !p.Category.IsDeleted && !p.Team.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                EnableTraking: false);

            if (project == null) throw new GlobalAppException("Layihə tapılmadı.");
            return _mapper.Map<ProjectDto>(project);
        }

        // READ (all)
        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            var list = await _projectRead.GetAllAsync(
                func: x => !x.IsDeleted && !x.Category.IsDeleted && !x.Team.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                orderBy: q => q.OrderBy(p => p.DisplayOrderId).ThenBy(p => p.CreatedDate),
                EnableTraking: false
            );
            return _mapper.Map<List<ProjectDto>>(list);
        }

        // READ (by category)
        public async Task<List<ProjectDto>> GetProjectsByCategoryAsync(string categoryId)
        {
            if (!Guid.TryParse(categoryId, out var catId))
                throw new GlobalAppException("Category ID formatı yanlışdır.");

            var list = await _projectRead.GetAllAsync(
                func: p => !p.IsDeleted && p.CategoryId == catId && !p.Category.IsDeleted && !p.Team.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                orderBy: q => q.OrderBy(p => p.DisplayOrderId).ThenBy(p => p.CreatedDate),
                EnableTraking: false
            );

            return _mapper.Map<List<ProjectDto>>(list);
        }

        // UPDATE (DisplayOrderId DƏYİŞMİR)
        public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto)
        {
            if (dto == null || !Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _projectRead.GetAsync(
                p => p.Id == id && !p.IsDeleted,
                include: q => q
                    .Include(p => p.Category)
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
                EnableTraking: true) ?? throw new GlobalAppException("Layihə tapılmadı.");

            // DELETE images by Name
            if (dto.DeleteImageNames?.Any() == true && entity.ProjectImages != null)
            {
                var set = new HashSet<string>(dto.DeleteImageNames, StringComparer.OrdinalIgnoreCase);
                var toDelete = entity.ProjectImages.Where(i => !i.IsDeleted && set.Contains(i.Name)).ToList();
                foreach (var img in toDelete)
                {
                    await _cloudinaryService.DeleteFileAsync(img.Name);
                    img.IsDeleted = true;
                    img.DeletedDate = DateTime.UtcNow;
                    img.LastUpdatedDate = DateTime.UtcNow;
                    await _imageWrite.UpdateAsync(img);
                }
            }

            // DELETE videos by Name (file saxlanmış videolar)
            if (dto.DeleteVideoNames?.Any() == true && entity.ProjectVideos != null)
            {
                var set = new HashSet<string>(dto.DeleteVideoNames, StringComparer.OrdinalIgnoreCase);
                var toDelete = entity.ProjectVideos
                    .Where(v => !v.IsDeleted && !string.IsNullOrWhiteSpace(v.YoutubeLink) && set.Contains(v.YoutubeLink))
                    .ToList();

                foreach (var vid in toDelete)
                {
                    await _cloudinaryService.DeleteFileAsync(vid.YoutubeLink);
                    vid.IsDeleted = true;
                    vid.DeletedDate = DateTime.UtcNow;
                    vid.LastUpdatedDate = DateTime.UtcNow;
                    await _videoWrite.UpdateAsync(vid);
                }
            }

            // SCALARS (yalnız göndərilənlər)
            if (dto.Title != null) entity.Title = dto.Title;
            if (dto.TitleEng != null) entity.TitleEng = dto.TitleEng;
            if (dto.TitleRu != null) entity.TitleRu = dto.TitleRu;
            if (dto.SubTitle != null) entity.SubTitle = dto.SubTitle;
            if (dto.SubTitleEng != null) entity.SubTitleEng = dto.SubTitleEng;
            if (dto.SubTitleRu != null) entity.SubTitleRu = dto.SubTitleRu;

            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _cloudinaryService.DeleteFileAsync(entity.CardImage);
                entity.CardImage = await _cloudinaryService.UploadFileAsync(dto.CardImage);
            }

            // TeamId: boş string gəlirsə null et
            if (dto.TeamId != null)
                entity.TeamId = Guid.TryParse(dto.TeamId, out var newTeamId) ? newTeamId : (Guid?)null;

            if (!string.IsNullOrWhiteSpace(dto.CategoryId))
            {
                if (!Guid.TryParse(dto.CategoryId, out var newCatId))
                    throw new GlobalAppException("Category ID formatı yanlışdır.");
                entity.CategoryId = newCatId;
            }

            entity.LastUpdatedDate = DateTime.UtcNow;

            // ADD NEW images (Update => IFormFile)
            if (dto.ProjectImages?.Any() == true)
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

            // ADD NEW videos (Update => ProjectVideoInputDto: link və/və ya file)
            if (dto.ProjectVideos?.Any() == true)
            {
                foreach (var v in dto.ProjectVideos)
                {
                 

                    await _videoWrite.AddAsync(new ProjectVideo
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
            
                        YoutubeLink = v,
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

        // DELETE (soft)
        public async Task DeleteProjectAsync(string projectId)
        {
            if (!Guid.TryParse(projectId, out _))
                throw new GlobalAppException("Yanlış ID formatı.");

            var entity = await _projectRead.GetByIdAsync(projectId, EnableTraking: true);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Layihə tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;

            await _projectWrite.UpdateAsync(entity);
            await _projectWrite.CommitAsync();
        }

        // REORDER (kütləvi)
        public async Task BulkReorderAsync(List<ProjectReorderDto> items)
        {
            if (items == null || items.Count == 0) return;

            var idSet = new HashSet<Guid>();
            foreach (var it in items)
                if (Guid.TryParse(it.Id, out var gid)) idSet.Add(gid);

            var projects = await _projectRead.GetAllAsync(x => idSet.Contains(x.Id) && !x.IsDeleted);
            foreach (var p in projects)
            {
                var dto = items.First(i => Guid.TryParse(i.Id, out var gid) && gid == p.Id);
                p.DisplayOrderId = dto.DisplayOrderId;
                p.LastUpdatedDate = DateTime.UtcNow;
                await _projectWrite.UpdateAsync(p);
            }
            await _projectWrite.CommitAsync();
        }
    }
}
