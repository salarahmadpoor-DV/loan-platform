using Matchi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matchi.Infrastructure.Persistence.Configurations;

public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
{
    public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
    {
        builder.ToTable("ConversationParticipants");

        builder.HasKey(x => x.Id).HasName("PK_ConversationParticipants");
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ConversationId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.JoinedAt)
            .IsRequired()
            .HasDefaultValueSql("sysutcdatetime()");


        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.ConversationId)
            .HasConstraintName("FK_ConversationParticipants_Conversations")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.User)
            .WithMany(x => x.ConversationParticipants)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_ConversationParticipants_Users")
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.ConversationId)
            .HasDatabaseName("IX_ConversationParticipants_ConversationId");
    }
}
