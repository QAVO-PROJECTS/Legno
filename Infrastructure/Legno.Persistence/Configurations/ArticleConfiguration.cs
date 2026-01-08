using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.SubTitle).HasMaxLength(300);
            builder.Property(x => x.AuthorName).HasMaxLength(150);
            builder.Property(x => x.AuthorImage).HasMaxLength(250);
            builder.Property(x => x.CardImage).HasMaxLength(250);

            builder.HasMany(x => x.Images)
                   .WithOne(x => x.Article)
                   .HasForeignKey(x => x.ArticleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
