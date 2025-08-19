using Legno.Application.Dtos.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Category
{
    public class CategoryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NameRu { get; set; }
        public string NameEng { get; set; }

        public List<ProjectDto>? Projects { get; set; }
    }
}
