using Legno.Domain.Entities.Common;

namespace Legno.Domain.Entities;

public class Setting:BaseEntity
{
    public string Key { get; set; }
    public string Name { get; set; }
    public string? Value { get; set; }
    public string? ValueEng { get; set; }
    public string? ValueRu{ get; set; }
    public string? ImageOne { get; set; }
    public string? ImageTwo { get; set; }
    
}