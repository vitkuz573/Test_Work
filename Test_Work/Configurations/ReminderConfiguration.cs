using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test_Work.Entities;

namespace Test_Work.Configurations;

public class ReminderConfiguration : IEntityTypeConfiguration<Reminder>
{
    public void Configure(EntityTypeBuilder<Reminder> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(r => r.ChatId)
            .IsRequired();

        builder.Property(r => r.ReminderDate)
            .IsRequired();

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.IsSent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(r => r.ChatId);
        builder.HasIndex(r => new { r.ReminderDate, r.IsSent });
    }
}