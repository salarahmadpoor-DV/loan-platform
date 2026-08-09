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
    public DbSet<User> Users => Set<User>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceQuestion> ServiceQuestions => Set<ServiceQuestion>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<ProviderService> ProviderServices => Set<ProviderService>();
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<BusinessProvider> BusinessProviders => Set<BusinessProvider>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<RequestAnswer> RequestAnswers => Set<RequestAnswer>();
    public DbSet<Introduction> Introductions => Set<Introduction>();
    public DbSet<Review> Reviews => Set<Review>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MatchiDbContext).Assembly);

        modelBuilder.Entity<LoanRequest>()
            .ToTable("LoanRequest");

        // Users table
        modelBuilder.Entity<User>()
            .ToTable("Users");

        // Service domain
        modelBuilder.Entity<ServiceCategory>()
            .ToTable("ServiceCategories");

        modelBuilder.Entity<Service>()
            .ToTable("Services");

        modelBuilder.Entity<ServiceQuestion>()
            .ToTable("ServiceQuestions");

        modelBuilder.Entity<QuestionOption>()
            .ToTable("QuestionOptions");

        modelBuilder.Entity<Provider>()
            .ToTable("Providers");

        modelBuilder.Entity<ProviderService>()
            .ToTable("ProviderServices");

        modelBuilder.Entity<Business>()
            .ToTable("Businesses");

        modelBuilder.Entity<BusinessProvider>()
            .ToTable("BusinessProviders");

        modelBuilder.Entity<ServiceRequest>()
            .ToTable("ServiceRequests");

        modelBuilder.Entity<RequestAnswer>()
            .ToTable("RequestAnswers");

        modelBuilder.Entity<Introduction>()
            .ToTable("Introductions");

        modelBuilder.Entity<Review>()
            .ToTable("Reviews");
    }
}