using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;


namespace Legno.Application.Abstracts.Repositories.UserProjects
{
    public interface IUserProjectReadRepository : IReadRepository<UserProject> { }
    public interface IUserProjectWriteRepository : IWriteRepository<UserProject> { }
}
