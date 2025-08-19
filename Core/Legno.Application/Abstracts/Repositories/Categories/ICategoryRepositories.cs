using Legno.Domain.Entities;

using Legno.Application.Abstracts.Repositories;

namespace Legno.Application.Abstracts.Repositories.Categories
{
    public interface ICategoryReadRepository : IReadRepository<Category> { }
    public interface ICategoryWriteRepository : IWriteRepository<Category> { }
}
