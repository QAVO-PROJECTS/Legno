using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.BusinessService
{
    public class CreateBusinessServiceDto
    {
        public string Name { get; set; }
        public string NameEng { get; set; }
        public string NameRu { get; set; }
        public IFormFile CardImage { get; set; }
    }
}
