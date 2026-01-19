using Legno.Domain.Entities.Common;

namespace Legno.Domain.Entities;

public class ContactBranch:BaseEntity
{
    public string Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public List<Contact>? Contacts { get; set; }
}