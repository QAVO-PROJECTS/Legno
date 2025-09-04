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
    public class ServiceSliderImageConfiguration : IEntityTypeConfiguration<ServiceSliderImage>
    {
        public void Configure(EntityTypeBuilder<ServiceSliderImage> b)
        {
            b.ToTable("ServiceSliderImages");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
        }
    }
}
