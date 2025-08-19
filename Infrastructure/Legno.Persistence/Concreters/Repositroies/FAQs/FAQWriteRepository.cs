using Legno.Application.Abstracts.Repositories.FAQs;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.FAQs
{
    public class FAQWriteRepository : WriteRepository<FAQ>, IFAQWriteRepository
    {
        public FAQWriteRepository(LegnoDbContext context) : base(context) { }
    }
}