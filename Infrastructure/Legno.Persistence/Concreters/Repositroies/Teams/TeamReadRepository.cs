using Legno.Application.Abstracts.Repositories.Teams;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Teams
{
    public class TeamReadRepository : ReadRepository<Team>, ITeamReadRepository
    {
        public TeamReadRepository(LegnoDbContext context) : base(context) { }
    }
}