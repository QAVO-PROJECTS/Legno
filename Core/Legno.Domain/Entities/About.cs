using Legno.Domain.Entities.Common;

namespace Legno.Domain.Entities;

public class About:BaseEntity
{
    public string? Description { get; set; }
    public string? CompanyCreateDate { get; set; }
}