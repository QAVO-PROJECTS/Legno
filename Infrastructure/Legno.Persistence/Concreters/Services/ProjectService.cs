using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories;
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
        private readonly IProjectSliderImageWriteRepository _sliderImageWrite;
        private readonly IProjectFabricReadRepository _fabricRead;
        private readonly IProjectFabricWriteRepository _fabricWrite;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public ProjectService(
            IProjectReadRepository projectRead,
            IProjectWriteRepository projectWrite,
            IProjectImageWriteRepository imageWrite,
            IProjectVideoWriteRepository videoWrite,
            CloudinaryService cloudinaryService,
            IMapper mapper,
            IProjectSliderImageWriteRepository sliderImageWrite,
            IProjectFabricReadRepository fabricRead,
            IProjectFabricWriteRepository fabricWrite,
            IFileService fileService)
        {
            _projectRead = projectRead;
            _projectWrite = projectWrite;
            _imageWrite = imageWrite;
            _videoWrite = videoWrite;
            _cloudinaryService = cloudinaryService;
            _mapper = mapper;
            _sliderImageWrite = sliderImageWrite;
            _fabricRead = fabricRead;
            _fabricWrite = fabricWrite;
            _fileService = fileService;
        }

        // CREATE
        public async Task<ProjectDto> AddProjectAsync(CreateProjectDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");
       


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
                // Fix for CS8601: Possible null reference assignment.
                CategoryName = dto.CategoryName ?? string.Empty,
           
                TeamId = Guid.TryParse(dto.TeamId, out var teamId) ? teamId : (Guid?)null,
                DisplayOrderId = maxDisplay + 1,
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            // Card image
            if (dto.CardImage != null)
                entity.CardImage = await _fileService.UploadFile(dto.CardImage, "projects/cards");
            if (dto.EmployeeImage != null)
                entity.EmployeeImage = await _fileService.UploadFile(dto.EmployeeImage, "projects/employees");

            await _projectWrite.AddAsync(entity);
            if (dto.FabricIds?.Any() == true)
            {
                foreach (var fidStr in dto.FabricIds)
                {
                    if (!Guid.TryParse(fidStr, out var fid)) continue;
                    await _fabricWrite.AddAsync(new ProjectFabric
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        FabricId = fid,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
            }
            // 📂 Project Images
            if (dto.ProjectImages?.Any() == true)
            {
                foreach (var file in dto.ProjectImages)
                {
                    var fileName = await _fileService.UploadFile(file, "projects/images");
                    await _imageWrite.AddAsync(new ProjectImage
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        Name = fileName,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }
            if (dto.ProjectSliderImages?.Any() == true)
            {
                foreach (var file in dto.ProjectSliderImages)
                {
                    var fileName = await _fileService.UploadFile(file, "projects/sliders");
                    await _sliderImageWrite.AddAsync(new ProjectSliderImage
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        Name = fileName,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }

            // 📺 Videos (sadəcə link saxlayırıq)
            if (dto.ProjectVideos?.Any() == true)
            {
                foreach (var v in dto.ProjectVideos)
                {
                    if (string.IsNullOrWhiteSpace(v)) continue;
                    await _videoWrite.AddAsync(new ProjectVideo
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        YoutubeLink = v.Trim(),
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }

            await _fabricWrite.CommitAsync();
            await _imageWrite.CommitAsync();
            await _sliderImageWrite.CommitAsync();
            await _videoWrite.CommitAsync();
            await _projectWrite.CommitAsync();

            var created = await _projectRead.GetAsync(
                p => p.Id == entity.Id && !p.IsDeleted,
                include: q => q
              
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
                p => p.Id == id
                     && !p.IsDeleted
              
                     && (p.TeamId == null || (p.Team != null && !p.Team.IsDeleted)),
                include: q => q
   
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectSliderImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted))
                    .Include(p => p.ProjectFabrics.Where(x => !x.IsDeleted))
                        .ThenInclude(pf => pf.Fabric),
                EnableTraking: false);

            if (project == null) throw new GlobalAppException("Layihə tapılmadı.");
            return _mapper.Map<ProjectDto>(project);
        }

        // READ (all)
        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            var list = await _projectRead.GetAllAsync(
                func: x => !x.IsDeleted
                        
                           && (x.TeamId == null || (x.Team != null && !x.Team.IsDeleted)),
                include: q => q
        
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectSliderImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted))
                    .Include(p => p.ProjectFabrics.Where(x => !x.IsDeleted))
                        .ThenInclude(pf => pf.Fabric),
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
                func: p => !p.IsDeleted
                        
                       
                           && (p.TeamId == null || (p.Team != null && !p.Team.IsDeleted)),
                include: q => q
               
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectSliderImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted))
                    .Include(p => p.ProjectFabrics.Where(x => !x.IsDeleted))
                        .ThenInclude(pf => pf.Fabric),
                orderBy: q => q.OrderBy(p => p.DisplayOrderId).ThenBy(p => p.CreatedDate),
                EnableTraking: false
            );

            return _mapper.Map<List<ProjectDto>>(list);
        }

        // UPDATE (DisplayOrderId DƏYİŞMİR)
        // UPDATE (DisplayOrderId DƏYİŞMİR)
        public async Task<ProjectDto> UpdateProjectAsync(UpdateProjectDto dto)
        {
            if (dto == null || !Guid.TryParse(dto.Id, out var id))
                throw new GlobalAppException("Yanlış ID.");

            var entity = await _projectRead.GetAsync(
                p => p.Id == id && !p.IsDeleted,
                include: q => q
             
                    .Include(p => p.Team)
                    .Include(p => p.ProjectImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectSliderImages.Where(i => !i.IsDeleted))
                    .Include(p => p.ProjectVideos.Where(v => !v.IsDeleted)),
          
                EnableTraking: true) ?? throw new GlobalAppException("Layihə tapılmadı.");

            // DELETE images by Name
            if (dto.DeleteImageNames?.Any() == true && entity.ProjectImages != null)
            {
                var set = new HashSet<string>(dto.DeleteImageNames, StringComparer.OrdinalIgnoreCase);
                var toDelete = entity.ProjectImages.Where(i => !i.IsDeleted && set.Contains(i.Name)).ToList();
                foreach (var img in toDelete)
                {
                    await _fileService.DeleteFile("projects/images", img.Name);
                    img.IsDeleted = true;
                    img.DeletedDate = DateTime.UtcNow;
                    img.LastUpdatedDate = DateTime.UtcNow;
                    await _imageWrite.UpdateAsync(img);
                }
            }

            // DELETE slider images by Name
            if (dto.DeleteSliderImageNames?.Any() == true && entity.ProjectSliderImages != null)
            {
                var set = new HashSet<string>(dto.DeleteSliderImageNames, StringComparer.OrdinalIgnoreCase);
                var toDelete = entity.ProjectSliderImages.Where(i => !i.IsDeleted && set.Contains(i.Name)).ToList();
                foreach (var img in toDelete)
                {
                    await _fileService.DeleteFile("projects/sliders",img.Name);
                    img.IsDeleted = true;
                    img.DeletedDate = DateTime.UtcNow;
                    img.LastUpdatedDate = DateTime.UtcNow;
                    await _sliderImageWrite.UpdateAsync(img);
                }
            }

            // DELETE videos by "YoutubeLink" (link-based)
            if (dto.DeleteVideoNames?.Any() == true && entity.ProjectVideos != null)
            {
                var set = new HashSet<string>(dto.DeleteVideoNames, StringComparer.OrdinalIgnoreCase);
                var toDelete = entity.ProjectVideos
                    .Where(v => !v.IsDeleted && !string.IsNullOrWhiteSpace(v.YoutubeLink) && set.Contains(v.YoutubeLink))
                    .ToList();

                foreach (var vid in toDelete)
                {
                    // Linklər Cloudinary faylı deyil; fayl saxlanılmırsa, DeleteFileAsync çağırmağa ehtiyac yoxdur.
                    vid.IsDeleted = true;
                    vid.DeletedDate = DateTime.UtcNow;
                    vid.LastUpdatedDate = DateTime.UtcNow;
                    await _videoWrite.UpdateAsync(vid);
                }
            }

            // SCALARS
            if (dto.Title != null) entity.Title = dto.Title;
            if (dto.TitleEng != null) entity.TitleEng = dto.TitleEng;
            if (dto.TitleRu != null) entity.TitleRu = dto.TitleRu;
            if (dto.SubTitle != null) entity.SubTitle = dto.SubTitle;
            if (dto.SubTitleEng != null) entity.SubTitleEng = dto.SubTitleEng;
            if (dto.SubTitleRu != null) entity.SubTitleRu = dto.SubTitleRu;
            if (dto.CategoryName != null) entity.CategoryName = dto.CategoryName;


            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _fileService.DeleteFile("projects/cards", entity.CardImage);

                entity.CardImage = await _fileService.UploadFile(dto.CardImage, "projects/cards");
            }

            if (dto.EmployeeImage != null)
            {

                await _fileService.DeleteFile("projects/employees", entity.EmployeeImage);

                entity.EmployeeImage = await _fileService.UploadFile(dto.EmployeeImage, "projects/employees");
            }

            // TeamId: boş string gələrsə null
            if (dto.TeamId != null)
                entity.TeamId = Guid.TryParse(dto.TeamId, out var newTeamId) ? newTeamId : (Guid?)null;



            entity.LastUpdatedDate = DateTime.UtcNow;

            // ADD NEW images
            if (dto.ProjectImages?.Any() == true)
            {
                foreach (var file in dto.ProjectImages)
                {
                    var fileName = await _fileService.UploadFile(file, "projects/images");
                    await _imageWrite.AddAsync(new ProjectImage
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        Name = fileName,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }

            if (dto.ProjectSliderImages?.Any() == true)
            {
                foreach (var file in dto.ProjectSliderImages)
                {
                    var fileName = await _fileService.UploadFile(file, "projects/sliders");
                    await _sliderImageWrite.AddAsync(new ProjectSliderImage
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        Name = fileName,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }

            // 📺 Yeni videolar
            if (dto.ProjectVideos?.Any() == true)
            {
                foreach (var v in dto.ProjectVideos)
                {
                    if (string.IsNullOrWhiteSpace(v)) continue;
                    await _videoWrite.AddAsync(new ProjectVideo
                    {
                        Id = Guid.NewGuid(),
                        ProjectId = entity.Id,
                        YoutubeLink = v.Trim(),
                        CreatedDate = DateTime.UtcNow,
                        LastUpdatedDate = DateTime.UtcNow
                    });
                }
            }

            // FABRICS: DELETE selected
            if (dto.DeleteFabricIds?.Any() == true)
            {
                var delIds = dto.DeleteFabricIds
                    .Select(s => Guid.TryParse(s, out var g) ? g : (Guid?)null)
                    .Where(g => g.HasValue).Select(g => g!.Value).ToHashSet();
                var existing = await _fabricRead.GetAllAsync(
                    pf => pf.ProjectId == entity.Id && !pf.IsDeleted && delIds.Contains(pf.FabricId),
                    EnableTraking: false);

                foreach (var pf in existing)
                {
                    pf.IsDeleted = true;
                    pf.DeletedDate = DateTime.UtcNow;
                    pf.LastUpdatedDate = DateTime.UtcNow;
                    await _fabricWrite.UpdateAsync(pf);
                }
            }

            // FABRICS: ADD new (skip if exists)
            if (dto.FabricIds?.Any() == true)
            {
                var addIds = dto.FabricIds
                    .Select(s => Guid.TryParse(s, out var g) ? g : (Guid?)null)
                    .Where(g => g.HasValue).Select(g => g!.Value).ToList();

                if (addIds.Count > 0)
                {
                    var existingAll = await _fabricRead.GetAllAsync(pf =>
                        pf.ProjectId == entity.Id && !pf.IsDeleted);

                    var existingSet = existingAll.Select(pf => pf.FabricId).ToHashSet();

                    foreach (var fid in addIds)
                    {
                        if (existingSet.Contains(fid)) continue;
                        await _fabricWrite.AddAsync(new ProjectFabric
                        {
                            Id = Guid.NewGuid(),
                            ProjectId = entity.Id,
                            FabricId = fid,
                            CreatedDate = DateTime.UtcNow,
                            LastUpdatedDate = DateTime.UtcNow,
                            IsDeleted = false
                        });
                    }
                }
            }

            await _projectWrite.UpdateAsync(entity);

            // Commits
            await _fabricWrite.CommitAsync();
            await _imageWrite.CommitAsync();
            await _sliderImageWrite.CommitAsync();
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
