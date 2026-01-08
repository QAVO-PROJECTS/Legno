using System;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.NameEng).IsRequired().HasMaxLength(150);
            builder.Property(x => x.NameRu).IsRequired().HasMaxLength(150);

            builder.HasMany(x => x.CategorySliderImages)
       .WithOne(x => x.Category)
       .HasForeignKey(x => x.CategoryId)
       .OnDelete(DeleteBehavior.Cascade);
            // Soft-delete global filter
            builder.HasQueryFilter(x => !x.IsDeleted);


        }
    }
}