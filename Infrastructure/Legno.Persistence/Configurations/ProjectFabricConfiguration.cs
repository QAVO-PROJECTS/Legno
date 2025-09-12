using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class ProjectFabricConfiguration : IEntityTypeConfiguration<ProjectFabric>
    {
        public void Configure(EntityTypeBuilder<ProjectFabric> builder)
        {
            builder.HasKey(pf => pf.Id); // BaseEntity-də Id varsa

            // Əlaqələr
            builder.HasOne(pf => pf.Project)
                   .WithMany(p => p.ProjectFabrics)
                   .HasForeignKey(pf => pf.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pf => pf.Fabric)
                   .WithMany(f => f.ProjectFabrics)
                   .HasForeignKey(pf => pf.FabricId)
                   .OnDelete(DeleteBehavior.Cascade);

    
        }
    }
}
