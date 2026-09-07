using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matchi.Infrastructure.Persistence;

public sealed class MatchiDbContext : DbContext
{
    public MatchiDbContext(DbContextOptions<MatchiDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<BusinessProvider> BusinessProviders => Set<BusinessProvider>();

    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceAttribute> ServiceAttributes => Set<ServiceAttribute>();
    public DbSet<ServiceAttributeOption> ServiceAttributeOptions => Set<ServiceAttributeOption>();

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<ProductAttributeOption> ProductAttributeOptions => Set<ProductAttributeOption>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();

    public DbSet<ProviderService> ProviderServices => Set<ProviderService>();
    public DbSet<BusinessService> BusinessServices => Set<BusinessService>();
    public DbSet<ProviderCapability> ProviderCapabilities => Set<ProviderCapability>();
    public DbSet<ProviderProduct> ProviderProducts => Set<ProviderProduct>();
    public DbSet<BusinessProduct> BusinessProducts => Set<BusinessProduct>();

    public DbSet<ProviderServiceArea> ProviderServiceAreas => Set<ProviderServiceArea>();
    public DbSet<BusinessServiceArea> BusinessServiceAreas => Set<BusinessServiceArea>();
    public DbSet<ProviderAvailability> ProviderAvailabilities => Set<ProviderAvailability>();
    public DbSet<BusinessAvailability> BusinessAvailabilities => Set<BusinessAvailability>();

    public DbSet<Request> Requests => Set<Request>();
    public DbSet<RequestService> RequestServices => Set<RequestService>();
    public DbSet<RequestProduct> RequestProducts => Set<RequestProduct>();
    public DbSet<RequestServiceAttribute> RequestServiceAttributes => Set<RequestServiceAttribute>();
    public DbSet<RequestProductAttribute> RequestProductAttributes => Set<RequestProductAttribute>();
    public DbSet<RequestLocation> RequestLocations => Set<RequestLocation>();
    public DbSet<RequestSchedule> RequestSchedules => Set<RequestSchedule>();

    public DbSet<Proposal> Proposals => Set<Proposal>();
    public DbSet<ProposalItem> ProposalItems => Set<ProposalItem>();

    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<ServiceExecution> ServiceExecutions => Set<ServiceExecution>();
    public DbSet<ExecutionAssignment> ExecutionAssignments => Set<ExecutionAssignment>();
    public DbSet<ProductDelivery> ProductDeliveries => Set<ProductDelivery>();

    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<Cancellation> Cancellations => Set<Cancellation>();

    public DbSet<Media> Media => Set<Media>();
    public DbSet<BusinessPortfolio> BusinessPortfolios => Set<BusinessPortfolio>();
    public DbSet<BusinessPortfolioMedia> BusinessPortfolioMedia => Set<BusinessPortfolioMedia>();
    public DbSet<ProviderPortfolio> ProviderPortfolios => Set<ProviderPortfolio>();
    public DbSet<ProviderPortfolioMedia> ProviderPortfolioMedia => Set<ProviderPortfolioMedia>();

    public DbSet<Verification> Verifications => Set<Verification>();
    public DbSet<VerificationDocument> VerificationDocuments => Set<VerificationDocument>();
    public DbSet<TrustScore> TrustScores => Set<TrustScore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MatchiDbContext).Assembly);
    }
}
