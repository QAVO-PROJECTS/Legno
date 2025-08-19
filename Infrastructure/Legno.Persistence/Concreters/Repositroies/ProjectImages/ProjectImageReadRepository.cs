using Legno.Application.Abstracts.Repositories.ProjectImages;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.ProjectImages
{
    public class ProjectImageReadRepository : ReadRepository<ProjectImage>, IProjectImageReadRepository
    {
        public ProjectImageReadRepository(LegnoDbContext context) : base(context) { }
    }
}