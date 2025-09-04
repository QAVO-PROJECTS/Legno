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

    public class BusinessServiceConfiguration : IEntityTypeConfiguration<BusinessService>
    {
        public void Configure(EntityTypeBuilder<BusinessService> b)
        {
            b.ToTable("BusinessServices");
            b.HasKey(x => x.Id);
        }
    }
}
