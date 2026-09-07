using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(x => x.Id).HasName("PK_Messages");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ConversationId)
            .IsRequired();

        builder.Property(x => x.SenderUserId)
            .IsRequired();

        builder.Property(x => x.Text)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ConversationId)
            .HasConstraintName("FK_Messages_Conversations")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.SenderUser)
            .WithMany(x => x.SentMessages)
            .HasForeignKey(x => x.SenderUserId)
            .HasConstraintName("FK_Messages_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => new { x.ConversationId, x.CreatedAt })
            .HasDatabaseName("IX_Messages_ConversationId_CreatedAt");
    }
}
