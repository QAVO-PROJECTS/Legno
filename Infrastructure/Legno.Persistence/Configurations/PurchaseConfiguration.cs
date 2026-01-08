using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.Subtitle).HasMaxLength(250);
            builder.Property(x => x.ProductOrService).HasMaxLength(200);
            builder.Property(x => x.Email).HasMaxLength(120);
            builder.Property(x => x.PhoneNumber).HasMaxLength(50);
            builder.Property(x => x.FileName).HasMaxLength(250);
        }
    }
}
