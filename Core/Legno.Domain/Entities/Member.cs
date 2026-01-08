using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class Member:BaseEntity
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string? TitleEng {  get; set; }
        public string? TitleRu { get; set; }
    }
}
