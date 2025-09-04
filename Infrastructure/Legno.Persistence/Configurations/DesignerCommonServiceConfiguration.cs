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
    public class DesignerCommonServiceConfiguration : IEntityTypeConfiguration<DesignerCommonService>
    {
        public void Configure(EntityTypeBuilder<DesignerCommonService> b)
        {
            b.ToTable("DesignerCommonServices");
            b.HasKey(x => x.Id);
        }
    }
}
