using System;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    // Team domain class has no Id; map a shadow key to persist.
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Teams");

            // Shadow primary key
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NameEng).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NameRu).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Surname).IsRequired().HasMaxLength(100);
            builder.Property(x => x.SurnameEng).IsRequired().HasMaxLength(100);
            builder.Property(x => x.SurnameRu).IsRequired().HasMaxLength(100);

            builder.Property(x => x.Position).IsRequired().HasMaxLength(150);
            builder.Property(x => x.PositionEng).IsRequired().HasMaxLength(150);
            builder.Property(x => x.PositionRu).IsRequired().HasMaxLength(150);

            builder.Property(x => x.CardImage).IsRequired().HasMaxLength(300);
            builder.Property(x => x.InstagramLink).HasMaxLength(300);
            builder.Property(x => x.LinkedInLink).HasMaxLength(300);
        }
    }
}