using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;


namespace Legno.Application.Abstracts.Repositories.ProjectVideos
{
    public interface IProjectVideoReadRepository : IReadRepository<ProjectVideo> { }
    public interface IProjectVideoWriteRepository : IWriteRepository<ProjectVideo> { }
}
