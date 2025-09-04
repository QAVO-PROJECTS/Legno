using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class ProjectVideoConfiguration : IEntityTypeConfiguration<ProjectVideo>
    {
        public void Configure(EntityTypeBuilder<ProjectVideo> builder)
        {
            builder.ToTable("ProjectVideos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.YoutubeLink).IsRequired().HasMaxLength(300);
            builder.Property(x => x.ProjectId).IsRequired();

            builder.HasOne(x => x.Project)
                   .WithMany(x => x.ProjectVideos)
                   .HasForeignKey(x => x.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}