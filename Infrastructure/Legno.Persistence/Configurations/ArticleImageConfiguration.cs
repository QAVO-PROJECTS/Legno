using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class ArticleImageConfiguration : IEntityTypeConfiguration<ArticleImage>
    {
        public void Configure(EntityTypeBuilder<ArticleImage> builder)
        {
            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(250);
        }
    }
}
