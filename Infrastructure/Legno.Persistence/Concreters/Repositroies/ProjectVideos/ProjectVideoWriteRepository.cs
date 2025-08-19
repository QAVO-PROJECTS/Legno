using Legno.Application.Abstracts.Repositories.ProjectVideos;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.ProjectVideos
{
    public class ProjectVideoWriteRepository : WriteRepository<ProjectVideo>, IProjectVideoWriteRepository
    {
        public ProjectVideoWriteRepository(LegnoDbContext context) : base(context) { }
    }
}