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
    public class DesignerServiceConfiguration : IEntityTypeConfiguration<DesignerService>
    {
        public void Configure(EntityTypeBuilder<DesignerService> b)
        {
            b.ToTable("DesignerServices");
            b.HasKey(x => x.Id);
        }

    }
}
