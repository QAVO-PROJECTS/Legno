using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;


namespace Legno.Application.Abstracts.Repositories.Projects
{
    public interface IProjectReadRepository : IReadRepository<Project> { }
    public interface IProjectWriteRepository : IWriteRepository<Project> { }
}
