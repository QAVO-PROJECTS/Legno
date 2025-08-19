using Legno.Application.Abstracts.Repositories.Subscribers;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Subscribers
{
    public class SubscriberWriteRepository : WriteRepository<Subscriber>, ISubscriberWriteRepository
    {
        public SubscriberWriteRepository(LegnoDbContext context) : base(context) { }
    }
}