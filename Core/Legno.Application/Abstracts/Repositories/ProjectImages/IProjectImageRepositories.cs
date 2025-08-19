using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;


namespace Legno.Application.Abstracts.Repositories.ProjectImages
{
    public interface IProjectImageReadRepository : IReadRepository<ProjectImage> { }
    public interface IProjectImageWriteRepository : IWriteRepository<ProjectImage> { }
}
