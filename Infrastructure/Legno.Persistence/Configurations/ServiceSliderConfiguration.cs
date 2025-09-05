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
    public class ServiceSliderConfiguration : IEntityTypeConfiguration<ServiceSlider>
    {
        public void Configure(EntityTypeBuilder<ServiceSlider> b)
        {
            b.ToTable("ServiceSliders");
            b.HasKey(x => x.Id);
     
        }
    }
}
