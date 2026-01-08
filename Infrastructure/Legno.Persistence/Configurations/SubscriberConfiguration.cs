using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class SubscriberConfiguration : IEntityTypeConfiguration<Subscriber>
    {
        public void Configure(EntityTypeBuilder<Subscriber> builder)
        {
            builder.ToTable("Subscribers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                   .IsRequired()
                   .HasMaxLength(200);

            // ✅ SQL Server sintaksisi (BIT = 0 => false, 1 => true)
            builder.HasIndex(x => x.Email)
                   .IsUnique()
                   .HasFilter("[IsDeleted] = 0")
                   .HasDatabaseName("IX_Subscribers_Email_Active");

            // Global query filter (soft delete)
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
