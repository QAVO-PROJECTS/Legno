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
    public class CategoryImageConfiguration : IEntityTypeConfiguration<CategoryImage>
    {
        public void Configure(EntityTypeBuilder<CategoryImage> builder)
        {
            builder.ToTable("CategorySliderImages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(300);

            builder.Property(x => x.CategoryId).IsRequired();

            builder.HasOne(x => x.Category)
                   .WithMany(x => x.CategorySliderImages)
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
