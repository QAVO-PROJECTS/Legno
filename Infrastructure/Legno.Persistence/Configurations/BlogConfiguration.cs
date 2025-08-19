using System;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    // Blog domain class has no Id; map a shadow key so it can be persisted.
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> builder)
        {
            builder.ToTable("Blogs");

            // Shadow primary key
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TitleEng).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TitleRu).IsRequired().HasMaxLength(200);

            builder.Property(x => x.SubTitle).HasMaxLength(400);
            builder.Property(x => x.SubTitleEng).HasMaxLength(400);
            builder.Property(x => x.SubTitleRu).HasMaxLength(400);

            builder.Property(x => x.BlogImage).IsRequired().HasMaxLength(300);

            builder.Property(x => x.AuthorName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.AuthorNameEng).IsRequired().HasMaxLength(150);
            builder.Property(x => x.AuthorNameRu).IsRequired().HasMaxLength(150);

            builder.Property(x => x.AuthorImage).IsRequired().HasMaxLength(300);
        }
    }
}