using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class ProjectVideo:BaseEntity
    {

        public string YoutubeLink { get; set; }
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
