using System;
using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TitleEng).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TitleRu).IsRequired().HasMaxLength(200);

       

            builder.Property(x => x.CardImage).IsRequired().HasMaxLength(300);

            builder.Property(x => x.DisplayOrderId).HasDefaultValue(1);

            // Soft-delete
            builder.HasQueryFilter(x => !x.IsDeleted);

            // Relation to images/videos
            builder.HasMany(x => x.ProjectImages)
                   .WithOne(x => x.Project)
                   .HasForeignKey(x => x.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.Team)
                   .WithMany(x => x.Projects)
                   .HasForeignKey(x => x.TeamId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ProjectVideos)
                   .WithOne(x => x.Project)
                   .HasForeignKey(x => x.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

      

        }
    }
}