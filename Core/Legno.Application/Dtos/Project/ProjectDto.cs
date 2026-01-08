using Legno.Application.Dtos.Category;
using Legno.Application.Dtos.Fabric;
using Legno.Application.Dtos.Team;
using Legno.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Project
{
    public class ProjectDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TitleEng { get; set; }
        public string TitleRu { get; set; }
        public string? SubTitle { get; set; }
        public string? SubTitleEng { get; set; }
        public string? SubTitleRu { get; set; }
        public string CardImage { get; set; }
        public int DisplayOrderId { get; set; }
        public TeamDto Team { get; set; }
        public string? CategoryName { get; set; }
        public List<string>? ProjectImageNames { get; set; }
        public List<string>? ProjectSliderImages { get; set; }
        public List<string>? ProjectVideoNames { get; set; }
        public List<FabricDto>? Fabrics { get; set; }
    }
}
