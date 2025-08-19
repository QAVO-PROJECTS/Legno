using Legno.Application.Abstracts.Repositories;
using Legno.Domain.Entities;


namespace Legno.Application.Abstracts.Repositories.Contacts
{
    public interface IContactReadRepository : IReadRepository<Contact> { }
    public interface IContactWriteRepository : IWriteRepository<Contact> { }
}
