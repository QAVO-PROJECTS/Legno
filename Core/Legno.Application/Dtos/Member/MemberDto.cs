using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.Member
{
    public class MemberDto
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string? TitleEng { get; set; }
        public string? TitleRu { get; set; }
    }
}
