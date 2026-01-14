using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Application.Abstracts.Repositories.Teams;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Team;
using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class TeamsService : ITeamService
    {
        private readonly ITeamReadRepository _read;
        private readonly ITeamWriteRepository _write;
        private readonly IProjectReadRepository _projectRead;
        private readonly IProjectWriteRepository _projectWrite;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public TeamsService(
            ITeamReadRepository read,
            ITeamWriteRepository write,
            IProjectReadRepository projectRead,
            IProjectWriteRepository projectWrite,
            IFileService fileService,
            IMapper mapper)
        {
            _read = read;
            _write = write;
            _projectRead = projectRead;
            _projectWrite = projectWrite;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<TeamDto> AddTeamAsync(CreateTeamDto dto)
        {
            if (dto == null) throw new GlobalAppException("Məlumat göndərilməyib.");

            var all = await _read.GetAllAsync(t => !t.IsDeleted);
            var maxOrder = all.Any() ? all.Max(t => t.DisplayOrderId) : 0;

            var entity = _mapper.Map<Team>(dto);
            entity.Id = Guid.NewGuid();
            entity.DisplayOrderId = maxOrder + 1;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            if (dto.CardImage != null)
                entity.CardImage = await _fileService.UploadFile(dto.CardImage, "teams");

            await _write.AddAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<TeamDto>(entity);
        }

        public async Task<TeamDto?> GetTeamAsync(string id)
        {
            var entity = await _read.GetByIdAsync(id, EnableTraking: false);
            if (entity == null || entity.IsDeleted)
                throw new GlobalAppException("Komanda üzvü tapılmadı.");
            return _mapper.Map<TeamDto>(entity);
        }

        public async Task<List<TeamDto>> GetAllTeamsAsync()
        {
            var list = await _read.GetAllAsync(
                func: t => !t.IsDeleted,
                orderBy: q => q.OrderBy(t => t.DisplayOrderId),
                EnableTraking: false);

            return _mapper.Map<List<TeamDto>>(list);
        }
//salam
        public async Task<TeamDto> UpdateTeamAsync(UpdateTeamDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id))
                throw new GlobalAppException("Id tələb olunur.");

            var entity = await _read.GetByIdAsync(dto.Id, EnableTraking: true)
                ?? throw new GlobalAppException("Komanda tapılmadı.");

            _mapper.Map(dto, entity);

            if (dto.CardImage != null)
            {
                if (!string.IsNullOrWhiteSpace(entity.CardImage))
                    await _fileService.DeleteFile("teams", entity.CardImage);

                entity.CardImage = await _fileService.UploadFile(dto.CardImage, "teams");
            }

            entity.LastUpdatedDate = DateTime.UtcNow;
            await _write.UpdateAsync(entity);
            await _write.CommitAsync();

            return _mapper.Map<TeamDto>(entity);
        }

        public async Task DeleteTeamAsync(string id)
        {
            var entity = await _read.GetByIdAsync(id, EnableTraking: true)
                ?? throw new GlobalAppException("Komanda tapılmadı.");

            entity.IsDeleted = true;
            entity.DeletedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = DateTime.UtcNow;
            await _write.UpdateAsync(entity);

            var relatedProjects = await _projectRead.GetAllAsync(p => p.TeamId == entity.Id && !p.IsDeleted);
            foreach (var p in relatedProjects)
            {
                p.IsDeleted = true;
                p.DeletedDate = DateTime.UtcNow;
                p.LastUpdatedDate = DateTime.UtcNow;
                await _projectWrite.UpdateAsync(p);
            }

            await _write.CommitAsync();
            await _projectWrite.CommitAsync();
        }

        public async Task ReorderTeamsAsync(List<TeamOrderUpdateDto> orders)
        {
            if (orders == null || orders.Count == 0) return;

            var idMap = new Dictionary<Guid, int>();
            foreach (var o in orders)
            {
                if (!Guid.TryParse(o.TeamId, out var gid))
                    throw new GlobalAppException($"Yanlış ID: {o.TeamId}");
                idMap[gid] = o.DisplayOrderId;
            }

            var teams = await _read.GetAllAsync(t => idMap.Keys.Contains(t.Id) && !t.IsDeleted, EnableTraking: true);
            foreach (var t in teams)
            {
                t.DisplayOrderId = idMap[t.Id];
                t.LastUpdatedDate = DateTime.UtcNow;
                await _write.UpdateAsync(t);
            }

            await _write.CommitAsync();
        }
    }
}
