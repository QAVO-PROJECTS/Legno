using Legno.Application.Abstracts.Repositories.ProjectVideos;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.ProjectVideos
{
    public class ProjectVideoReadRepository : ReadRepository<ProjectVideo>, IProjectVideoReadRepository
    {
        public ProjectVideoReadRepository(LegnoDbContext context) : base(context) { }
    }
}