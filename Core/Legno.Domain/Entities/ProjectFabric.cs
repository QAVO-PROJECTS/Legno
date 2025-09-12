using Legno.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Domain.Entities
{
    public class ProjectFabric:BaseEntity
    {
        public Guid FabricId { get; set; }
        public Project Project { get; set; }
        public Guid ProjectId { get; set; }
        public Fabric Fabric { get; set; }
    }
}
