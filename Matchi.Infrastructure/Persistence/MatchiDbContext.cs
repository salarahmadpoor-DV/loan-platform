using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence;

public sealed class MatchiDbContext : DbContext
{
    public MatchiDbContext(DbContextOptions<MatchiDbContext> options)
        : base(options)
    {
    }

    #region DbSets

    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<LoanRequest> LoanRequests => Set<LoanRequest>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MatchiDbContext).Assembly);

        modelBuilder.Entity<LoanRequest>()
            .ToTable("LoanRequest");
    }
}