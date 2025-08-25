using Legno.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Project
{
    public class CreateProjectDto
    {
        public string Title { get; set; }
        public string TitleEng { get; set; }
        public string TitleRu { get; set; }
        public string? SubTitle { get; set; }
        public string? SubTitleEng { get; set; }
        public string? SubTitleRu { get; set; }
        public IFormFile CardImage { get; set; }
        public string AuthorName { get; set; }
        public string CategoryId { get; set; } // REQUIRED
        public List<IFormFile>? ProjectImages { get; set; }
        public List<IFormFile>? ProjectVideos { get; set; }
    }
}
