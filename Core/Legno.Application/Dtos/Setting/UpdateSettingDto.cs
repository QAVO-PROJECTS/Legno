using Microsoft.AspNetCore.Http;

namespace Legno.Application.Dtos.Setting;

public class UpdateSettingDto
{
    public string Id { get; set; }
    public string? Key { get; set; }
    public string? Name { get; set; }
    public string? Value { get; set; }
    public string? ValueEng { get; set; }
    public string? ValueRu{ get; set; }
    public IFormFile? ImageOne { get; set; }
    public IFormFile? ImageTwo { get; set; }
}