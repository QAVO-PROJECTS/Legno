using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;



namespace Legno.Application.Abstracts.Repositories.Blogs
{
    public interface IBlogReadRepository : IReadRepository<Blog> { }
    public interface IBlogWriteRepository : IWriteRepository<Blog> { }
}
