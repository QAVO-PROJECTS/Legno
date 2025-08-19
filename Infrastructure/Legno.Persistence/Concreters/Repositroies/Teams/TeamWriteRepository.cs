using Legno.Application.Abstracts.Repositories.Teams;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Teams
{
    public class TeamWriteRepository : WriteRepository<Team>, ITeamWriteRepository
    {
        public TeamWriteRepository(LegnoDbContext context) : base(context) { }
    }
}