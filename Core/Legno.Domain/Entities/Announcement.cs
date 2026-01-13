using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class Announcement:BaseEntity
    {
        public string Title { get; set; }
        public string TitleEng { get; set; }
        public string TitleRu { get; set; }


        public string SubTitle { get; set; }
        public string SubTitleEng { get; set; }
        public string SubTitleRu { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorNameEng { get; set; }
        public string? AuthorNameRu { get; set; }

        public string? AuthorImage { get; set; }
        public string? CardImage { get; set; }

    }
}
