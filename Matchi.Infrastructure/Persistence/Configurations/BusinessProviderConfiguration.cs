using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class BusinessProviderConfiguration : IEntityTypeConfiguration<BusinessProvider>
{
    public void Configure(EntityTypeBuilder<BusinessProvider> builder)
    {
        builder.ToTable("BusinessProviders");
        builder.HasKey(bp => bp.Id);
        builder.HasOne(bp => bp.Business).WithMany(b => b.BusinessProviders).HasForeignKey(bp => bp.BusinessId);
        builder.HasOne(bp => bp.Provider).WithMany().HasForeignKey(bp => bp.ProviderId);
    }
}