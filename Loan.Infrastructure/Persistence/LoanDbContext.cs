using Loan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loan.Infrastructure.Persistence;

public sealed class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options)
        : base(options)
    {
    }

    #region DbSets

    public DbSet<Bank> Banks => Set<Bank>();
    public DbSet<LoanRequest> LoanRequests => Set<LoanRequest>();
    
    // بعداً اضافه می‌شوند
    // public DbSet<BankQuestion> BankQuestions => Set<BankQuestion>();
    // public DbSet<BankQuestionOption> BankQuestionOptions => Set<BankQuestionOption>();
    // public DbSet<LoanRequest> LoanRequests => Set<LoanRequest>();
    // public DbSet<LoanRequestAnswer> LoanRequestAnswers => Set<LoanRequestAnswer>();

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanDbContext).Assembly);
        modelBuilder.Entity<LoanRequest>().ToTable("LoanRequest");
        modelBuilder.HasDefaultSchema("dbo");
    }

    
}