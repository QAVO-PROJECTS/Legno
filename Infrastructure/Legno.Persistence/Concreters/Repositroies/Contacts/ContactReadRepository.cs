using Legno.Application.Abstracts.Repositories.Contacts;
using Legno.Domain.Entities;
using Legno.Persistence.Context;

namespace Legno.Persistence.Concreters.Repositories.Contacts
{
    public class ContactReadRepository : ReadRepository<Contact>, IContactReadRepository
    {
        public ContactReadRepository(LegnoDbContext context) : base(context) { }
    }
}