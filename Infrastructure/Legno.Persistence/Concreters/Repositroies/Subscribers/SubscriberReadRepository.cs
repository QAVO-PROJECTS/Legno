using Legno.Application.Abstracts.Repositories.Subscribers;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Subscribers
{
    public class SubscriberReadRepository : ReadRepository<Subscriber>, ISubscriberReadRepository
    {
        public SubscriberReadRepository(LegnoDbContext context) : base(context) { }
    }
}