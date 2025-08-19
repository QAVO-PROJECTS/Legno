using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Projects
{
    public class ProjectReadRepository : ReadRepository<Project>, IProjectReadRepository
    {
        public ProjectReadRepository(LegnoDbContext context) : base(context) { }
    }
}