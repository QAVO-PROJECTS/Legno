using Legno.Domain.Entities.Common;

namespace Legno.Domain.Entities;

public class Director:BaseEntity
{
    public string Image { get; set; }
    public string Title { get; set; }
    public string? TitleEng {  get; set; }
    public string? TitleRu { get; set; }
}