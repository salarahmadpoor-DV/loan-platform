using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ProviderServiceConfiguration : IEntityTypeConfiguration<ProviderService>
{
    public void Configure(EntityTypeBuilder<ProviderService> builder)
    {
        builder.ToTable("ProviderServices");
        builder.HasKey(ps => ps.Id);
        builder.HasOne(ps => ps.Provider).WithMany(p => p.ProviderServices).HasForeignKey(ps => ps.ProviderId);
        builder.HasOne(ps => ps.Service).WithMany().HasForeignKey(ps => ps.ServiceId);
    }
}