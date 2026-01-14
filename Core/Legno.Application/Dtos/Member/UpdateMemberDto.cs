using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Member
{
    public class UpdateMemberDto
    {
        public string Id { get; set; }
        public IFormFile? Image { get; set; }
        public string? Title { get; set; }
        public string? TitleEng { get; set; }
        public string? TitleRu { get; set; }
        public string? JobTitle { get; set; }
        public string? JobTitleEng { get; set; }
        public string? JobTitleRu { get; set; }
        public string? InstagramLink { get; set; }
    }
}
