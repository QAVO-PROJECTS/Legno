using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class ContactBranchConfiguration : IEntityTypeConfiguration<ContactBranch>
    {
        public void Configure(EntityTypeBuilder<ContactBranch> builder)
        {
            builder.ToTable("ContactBranches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Address)
                .HasMaxLength(256);

            builder.Property(x => x.Phone)
                .HasMaxLength(32);

            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.HasMany(x => x.Contacts)
                .WithOne(x => x.ContactBranch)
                .HasForeignKey(x => x.ContactBranchId);

        }
    }
}