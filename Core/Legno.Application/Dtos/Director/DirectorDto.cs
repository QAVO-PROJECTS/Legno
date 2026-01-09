using Microsoft.AspNetCore.Http;

namespace Legno.Application.Dtos.Director;

public class DirectorDto
{
    public string Id { get; set; }
    public string Image { get; set; }
    public string Title { get; set; }
    public string? TitleEng {  get; set; }
    public string? TitleRu { get; set; }
}