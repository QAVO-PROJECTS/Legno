using Microsoft.AspNetCore.Http;

namespace Legno.Application.Dtos.Director;

public class UpdateDirectorDto
{
    public string Id { get; set; }
    public IFormFile? Image { get; set; }
    public string? Title { get; set; }
    public string? TitleEng {  get; set; }
    public string? TitleRu { get; set; }
}