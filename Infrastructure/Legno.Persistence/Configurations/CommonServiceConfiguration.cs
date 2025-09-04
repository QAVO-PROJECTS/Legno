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
    public class CommonServiceConfiguration : IEntityTypeConfiguration<CommonService>
    {
        public void Configure(EntityTypeBuilder<CommonService> b)
        {
            b.ToTable("CommonServices");
            b.HasKey(x => x.Id);
        }
    }
}
