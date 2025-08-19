using Legno.Application.Dtos.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Abstracts.Services
{
    public interface ITeamService
    {
        Task<TeamDto> AddTeamAsync(CreateTeamDto createTeamDto);
        Task<TeamDto?> GetTeamAsync(string teamId);
        Task<List<TeamDto>> GetAllTeamsAsync();
        Task<TeamDto> UpdateTeamAsync(UpdateTeamDto updateTeamDto);
        Task ReorderTeamsAsync(List<TeamOrderUpdateDto> orders);
        Task DeleteTeamAsync(string teamId);



    }
}
