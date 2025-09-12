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

            // SubscriberConfiguration.cs
            builder.HasIndex(x => x.Email)
                   .IsUnique()
                   .HasFilter("\"IsDeleted\" = FALSE")   // PostgreSQL üçün partial index
                   .HasDatabaseName("IX_Subscribers_Email_Active");


            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}