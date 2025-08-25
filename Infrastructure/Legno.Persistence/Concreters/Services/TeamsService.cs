using AutoMapper;
using Legno.Application.Absrtacts.Services;
using Legno.Application.Abstracts.Repositories.Teams;
using Legno.Application.Abstracts.Services;
using Legno.Application.Dtos.Team;

using Legno.Application.GlobalExceptionn;
using Legno.Domain.Entities;

namespace Legno.Persistence.Concreters.Services
{
    public class TeamsService : ITeamService
    {
        private readonly ITeamReadRepository _teamReadRepository;
        private readonly ITeamWriteRepository _teamWriteRepository;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly CloudinaryService _cloudinaryService;

        public TeamsService(
            ITeamReadRepository teamReadRepository,
            ITeamWriteRepository teamWriteRepository,
            IFileService fileService,
            IMapper mapper,
            CloudinaryService cloudinaryService)
        {
            _teamReadRepository = teamReadRepository;
            _teamWriteRepository = teamWriteRepository;
            _fileService = fileService;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<TeamDto> AddTeamAsync(CreateTeamDto createTeamDto)
        {
            if (!string.IsNullOrWhiteSpace(createTeamDto.Name) && createTeamDto.Name.Length > 100)
                throw new GlobalAppException("Ad maksimum 100 simvol ola bilər.");

            // Max(DisplayOrderId) + 1  (GetAllAsync ilə)
            var existing = await _teamReadRepository.GetAllAsync(
                func: t => !t.IsDeleted,
                include: null,
                orderBy: null,
                EnableTraking: false
            );
            var maxOrder = existing.Any() ? existing.Max(t => t.DisplayOrderId) : 0;

            var entity = _mapper.Map<Team>(createTeamDto);
            entity.DisplayOrderId = maxOrder + 1;
            entity.IsDeleted = false;
            entity.CreatedDate = DateTime.UtcNow;
            entity.LastUpdatedDate = entity.CreatedDate;

            if (createTeamDto.CardImage != null)
            {
                var fileName = await _cloudinaryService.UploadFileAsync(createTeamDto.CardImage);
                entity.CardImage = fileName;
            }

            await _teamWriteRepository.AddAsync(entity);
            await _teamWriteRepository.CommitAsync();

            return _mapper.Map<TeamDto>(entity);
        }

        public async Task<TeamDto?> GetTeamAsync(string teamId)
        {
            var team = await _teamReadRepository.GetByIdAsync(teamId, EnableTraking: false);
            if (team == null || team.IsDeleted)
                return null;

            return _mapper.Map<TeamDto>(team);
        }

        public async Task<List<TeamDto>> GetAllTeamsAsync()
        {
            var teams = await _teamReadRepository.GetAllAsync(
                func: t => !t.IsDeleted,
                include: null,
                orderBy: q => q.OrderBy(t => t.DisplayOrderId).ThenByDescending(t => t.CreatedDate),
                EnableTraking: false
            );

            return _mapper.Map<List<TeamDto>>(teams.ToList());
        }

        public async Task<TeamDto> UpdateTeamAsync(UpdateTeamDto updateTeamDto)
        {
            if (string.IsNullOrWhiteSpace(updateTeamDto.Id))
                throw new GlobalAppException("Id boş ola bilməz.");

            var team = await _teamReadRepository.GetByIdAsync(updateTeamDto.Id, EnableTraking: true);
            if (team == null || team.IsDeleted)
                throw new GlobalAppException("Komanda üzvü tapılmadı!");

            // Null olmayan sahələri kopyala
            _mapper.Map(updateTeamDto, team);
            // ---- Manual field-by-field update (null gəlməyənlər tətbiq olunur) ----
            if (updateTeamDto.Name != null) team.Name = updateTeamDto.Name;
            if (updateTeamDto.NameEng != null) team.NameEng = updateTeamDto.NameEng;
            if (updateTeamDto.NameRu != null) team.NameRu = updateTeamDto.NameRu;

            if (updateTeamDto.Surname != null) team.Surname = updateTeamDto.Surname;
            if (updateTeamDto.SurnameEng != null) team.SurnameEng = updateTeamDto.SurnameEng;
            if (updateTeamDto.SurnameRu != null) team.SurnameRu = updateTeamDto.SurnameRu;

            if (updateTeamDto.Position != null) team.Position = updateTeamDto.Position;
            if (updateTeamDto.PositionEng != null) team.PositionEng = updateTeamDto.PositionEng;
            if (updateTeamDto.PositionRu != null) team.PositionRu = updateTeamDto.PositionRu;

            if (updateTeamDto.InstagramLink != null) team.InstagramLink = updateTeamDto.InstagramLink;
            if (updateTeamDto.LinkedInLink != null) team.LinkedInLink = updateTeamDto.LinkedInLink;

            // Yeni şəkil gəlirsə — köhnəni sil, yenisini yaz
            if (updateTeamDto.CardImage != null)
            {
                if (!string.IsNullOrEmpty(team.CardImage))
                    await   _cloudinaryService.DeleteFileAsync(team.CardImage);

                team.CardImage = await _cloudinaryService.UploadFileAsync(updateTeamDto.CardImage);
            }

        
            team.LastUpdatedDate = DateTime.UtcNow;

            await _teamWriteRepository.UpdateAsync(team);
            await _teamWriteRepository.CommitAsync();

            return _mapper.Map<TeamDto>(team);
        }

        public async Task DeleteTeamAsync(string teamId)
        {
            var team = await _teamReadRepository.GetByIdAsync(teamId, EnableTraking: true);
            if (team == null || team.IsDeleted)
                throw new GlobalAppException("Komanda üzvü tapılmadı!");

            team.IsDeleted = true;
            team.DeletedDate = DateTime.UtcNow;
            team.LastUpdatedDate = DateTime.UtcNow;

            await _teamWriteRepository.UpdateAsync(team);
            await _teamWriteRepository.CommitAsync();
        }

        // TeamId + DisplayOrderId siyahısına görə sıralamanı dəyiş
        public async Task ReorderTeamsAsync(List<TeamOrderUpdateDto> orders)
        {
            if (orders == null || orders.Count == 0)
                return;

            var dupDisplay = orders.GroupBy(o => o.DisplayOrderId).Any(g => g.Count() > 1);
            if (dupDisplay)
                throw new GlobalAppException("DisplayOrderId dəyərləri təkrarlanmamalıdır.");

            // Id-ləri yığ
            var idMap = new Dictionary<Guid, int>();
            foreach (var o in orders)
            {
                if (!Guid.TryParse(o.TeamId, out var gid))
                    throw new GlobalAppException($"Yanlış TeamId: {o.TeamId}");
                idMap[gid] = o.DisplayOrderId;
            }

            // Yalnız verilənlərə aid olanları çək (GetAllAsync ilə)
            var affected = await _teamReadRepository.GetAllAsync(
                func: t => !t.IsDeleted && idMap.Keys.Contains(t.Id),
                include: null,
                orderBy: null,
                EnableTraking: true
            );

            foreach (var t in affected)
            {
                t.DisplayOrderId = idMap[t.Id];
                t.LastUpdatedDate = DateTime.UtcNow;
                await _teamWriteRepository.UpdateAsync(t);
            }

            await _teamWriteRepository.CommitAsync();
        }
    }
}
