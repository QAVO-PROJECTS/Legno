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

            // Soft-delete global filter
            builder.HasQueryFilter(x => !x.IsDeleted);

            // Relationship: Category (1) -> Projects (many) via shadow FK on Project
            builder.HasMany(x => x.Projects)
                   .WithOne()
                   .HasForeignKey("CategoryId")
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}