using Legno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Legno.Persistence.Configurations
{
    public class FAQConfiguration : IEntityTypeConfiguration<FAQ>
    {
        public void Configure(EntityTypeBuilder<FAQ> builder)
        {
            builder.ToTable("FAQs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Question).HasMaxLength(300);
            builder.Property(x => x.QuestionEng).HasMaxLength(300);
            builder.Property(x => x.QuestionRu).HasMaxLength(300);

            builder.Property(x => x.Answer).HasMaxLength(4000);
            builder.Property(x => x.AnswerEng).HasMaxLength(4000);
            builder.Property(x => x.AnswerRu).HasMaxLength(4000);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}