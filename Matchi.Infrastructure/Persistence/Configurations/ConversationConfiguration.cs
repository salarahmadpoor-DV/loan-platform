using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(x => x.Id).HasName("PK_Conversations");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.Property(x => x.RequestId)
            .IsRequired();


        builder.Property(x => x.CustomerId)
            .IsRequired();

        builder.Property(x => x.CreateDate)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.HasOne(x => x.Business)
            .WithMany(x => x.Conversations)
            .HasForeignKey(x => x.BusinessId)
            .HasConstraintName("FK_Conversations_Businesses")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Conversations)
            .HasForeignKey(x => x.CustomerId)
            .HasConstraintName("FK_Conversations_Customers")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Request)
            .WithMany(x => x.Conversations)
            .HasForeignKey(x => x.RequestId)
            .HasConstraintName("FK_Conversations_Requests")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.RequestId)
            .HasDatabaseName("IX_Conversations_RequestId");
    }
}
