using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;


namespace Legno.Application.Abstracts.Repositories.Subscribers
{
    public interface ISubscriberReadRepository : IReadRepository<Subscriber> { }
    public interface ISubscriberWriteRepository : IWriteRepository<Subscriber> { }
}
