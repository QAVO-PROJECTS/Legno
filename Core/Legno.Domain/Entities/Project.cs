using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Title { get; set; }
        public string TitleEng { get; set; }
        public string TitleRu { get; set; }
        public string? SubTitle { get; set; }
        public string? SubTitleEng { get; set; }
        public string? SubTitleRu { get; set; }
        public string CardImage { get; set; }

        public int DisplayOrderId { get; set; } = 1;
        public Guid? TeamId { get; set; }
        public Team? Team { get; set; }
        public string CategoryName { get; set; }
        public List<ProjectImage>? ProjectImages { get; set; }
        public List<ProjectVideo>? ProjectVideos { get; set; }
        public List<ProjectFabric>? ProjectFabrics { get; set; }
        public List<ProjectSliderImage>? ProjectSliderImages { get; set; }

    }
}
