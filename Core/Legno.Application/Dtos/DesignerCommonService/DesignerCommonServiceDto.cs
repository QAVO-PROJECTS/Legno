using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Application.Dtos.DesignerCommonService
{
    public class DesignerCommonServiceDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TitleEng { get; set; }
        public string TitleRu { get; set; }
        public string SubTitle { get; set; }
        public string SubTitleEng { get; set; }
        public string SubTitleRu { get; set; }
        public string? CardImage { get; set; }
    }
}
