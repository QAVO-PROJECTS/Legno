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
    public class B2BServiceConfiguration : IEntityTypeConfiguration<B2BService>
    {
        public void Configure(EntityTypeBuilder<B2BService> b)
        {
            b.ToTable("B2BServices");
            b.HasKey(x => x.Id);
        }
    }
}
