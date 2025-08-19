using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;


namespace Legno.Application.Abstracts.Repositories.Teams
{
    public interface ITeamReadRepository : IReadRepository<Team> { }
    public interface ITeamWriteRepository : IWriteRepository<Team> { }
}
