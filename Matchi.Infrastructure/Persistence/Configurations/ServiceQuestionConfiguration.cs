using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ServiceQuestionConfiguration : IEntityTypeConfiguration<ServiceQuestion>
{
    public void Configure(EntityTypeBuilder<ServiceQuestion> builder)
    {
        builder.ToTable("ServiceQuestions");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Text).IsRequired().HasMaxLength(1000);
        builder.HasOne(q => q.Service).WithMany(s => s.Questions).HasForeignKey(q => q.ServiceId);
    }
}