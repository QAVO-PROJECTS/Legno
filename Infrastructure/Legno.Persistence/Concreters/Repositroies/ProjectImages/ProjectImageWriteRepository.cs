using Legno.Application.Abstracts.Repositories.ProjectImages;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.ProjectImages
{
    public class ProjectImageWriteRepository : WriteRepository<ProjectImage>, IProjectImageWriteRepository
    {
        public ProjectImageWriteRepository(LegnoDbContext context) : base(context) { }
    }
}