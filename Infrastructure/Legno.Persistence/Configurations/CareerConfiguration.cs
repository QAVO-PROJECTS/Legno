using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class CareerConfiguration : IEntityTypeConfiguration<Career>
    {
        public void Configure(EntityTypeBuilder<Career> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Surname).IsRequired().HasMaxLength(100);
            builder.Property(x => x.BirthDate).HasMaxLength(50);
            builder.Property(x => x.WorkExperience).HasMaxLength(500);
            builder.Property(x => x.Email).HasMaxLength(120);
            builder.Property(x => x.PhoneNumber).HasMaxLength(50);
            builder.Property(x => x.FileName).HasMaxLength(250);
        }
    }
}
