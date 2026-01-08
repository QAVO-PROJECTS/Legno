using Legno.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string NameRu { get; set; }
        public string NameEng { get; set; }

        public IFormFile CategoryImage { get; set; }
        public List<IFormFile>? CategorySliderImages { get; set; }
    }
}
