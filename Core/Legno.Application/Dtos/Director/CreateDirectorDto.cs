using Microsoft.AspNetCore.Http;

namespace Legno.Application.Dtos.Director;

public class CreateDirectorDto
{
    public IFormFile Image { get; set; }
    public string Title { get; set; }
    public string? TitleEng {  get; set; }
    public string? TitleRu { get; set; }
}