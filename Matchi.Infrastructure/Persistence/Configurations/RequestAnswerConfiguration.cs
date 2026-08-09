using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class RequestAnswerConfiguration : IEntityTypeConfiguration<RequestAnswer>
{
    public void Configure(EntityTypeBuilder<RequestAnswer> builder)
    {
        builder.ToTable("RequestAnswers");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Text).HasMaxLength(2000);
        builder.HasOne(a => a.ServiceRequest).WithMany(r => r.Answers).HasForeignKey(a => a.ServiceRequestId);
        builder.HasOne(a => a.SelectedOption).WithMany().HasForeignKey("SelectedOptionId").IsRequired(false);
    }
}