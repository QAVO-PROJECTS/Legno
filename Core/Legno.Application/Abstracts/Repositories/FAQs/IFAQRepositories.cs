using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;


namespace Legno.Application.Abstracts.Repositories.FAQs
{
    public interface IFAQReadRepository : IReadRepository<FAQ> { }
    public interface IFAQWriteRepository : IWriteRepository<FAQ> { }
}
