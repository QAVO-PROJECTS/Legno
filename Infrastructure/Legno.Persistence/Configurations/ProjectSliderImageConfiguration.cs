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
    public class ProjectSliderImageConfiguration : IEntityTypeConfiguration<ProjectSliderImage>
    {
        public void Configure(EntityTypeBuilder<ProjectSliderImage> builder)
        {
            builder.ToTable("ProjectSliderImages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(300);

            builder.Property(x => x.ProjectId).IsRequired();

            builder.HasOne(x => x.Project)
                   .WithMany(x => x.ProjectSliderImages)
                   .HasForeignKey(x => x.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
