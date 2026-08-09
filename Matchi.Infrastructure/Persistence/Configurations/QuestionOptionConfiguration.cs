using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Text).IsRequired().HasMaxLength(500);
        builder.HasOne(o => o.ServiceQuestion).WithMany(q => q.Options).HasForeignKey(o => o.ServiceQuestionId);
    }
}