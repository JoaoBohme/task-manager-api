using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(task => task.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(task => task.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(task => task.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(task => task.DateCreated)
            .IsRequired();

        builder.Property(task => task.DateCompleted);

        builder.Property(task => task.UserId)
            .IsRequired();

        builder.HasIndex(task => new { task.UserId, task.Status })
            .HasDatabaseName("IX_Tasks_UserId_Status");

        builder.HasIndex(task => new { task.Status, task.Priority, task.DateCreated })
            .HasDatabaseName("IX_Tasks_Status_Priority_DateCreated");
    }
}
