using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legno.Persistence.Configurations
{
    public class WorkPlanningConfiguration : IEntityTypeConfiguration<WorkPlanning>
    {
        public void Configure(EntityTypeBuilder<WorkPlanning> b)
        {
            b.ToTable("WorkPlannings");
            b.HasKey(x => x.Id);
        }
    }
}
