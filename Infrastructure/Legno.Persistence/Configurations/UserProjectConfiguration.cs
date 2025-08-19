using System;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    // UserProject has no Id; map a shadow key.
    public class UserProjectConfiguration : IEntityTypeConfiguration<UserProject>
    {
        public void Configure(EntityTypeBuilder<UserProject> builder)
        {
            builder.ToTable("UserProjects");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ProjectFileName).IsRequired().HasMaxLength(300);
            builder.Property(x => x.PhoneNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(2000);
        }
    }
}