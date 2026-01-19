using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("Settings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Key)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.Value)
         ; 

        builder.Property(x => x.ValueEng); 
        builder.Property(x => x.ValueRu);  

        builder.Property(x => x.ImageOne);
        builder.Property(x => x.ImageTwo);

        builder.HasIndex(x => x.Key).IsUnique();
    }
}