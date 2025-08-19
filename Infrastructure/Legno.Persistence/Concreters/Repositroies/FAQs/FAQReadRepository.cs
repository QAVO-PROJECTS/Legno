using Legno.Application.Abstracts.Repositories.FAQs;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.FAQs
{
    public class FAQReadRepository : ReadRepository<FAQ>, IFAQReadRepository
    {
        public FAQReadRepository(LegnoDbContext context) : base(context) { }
    }
}