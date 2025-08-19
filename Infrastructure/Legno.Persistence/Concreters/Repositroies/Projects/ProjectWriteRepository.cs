using Legno.Application.Abstracts.Repositories.Projects;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Projects
{
    public class ProjectWriteRepository : WriteRepository<Project>, IProjectWriteRepository
    {
        public ProjectWriteRepository(LegnoDbContext context) : base(context) { }
    }
}