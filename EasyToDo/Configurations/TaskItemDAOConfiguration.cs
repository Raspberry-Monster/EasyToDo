using EasyToDo.Models.DAO;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Configurations
{
    public class TaskItemDAOConfiguration : IEntityTypeConfiguration<TaskItemDAO>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TaskItemDAO> builder)
        {
            builder.ToTable("TaskItems");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("Id");

            builder.Property(t => t.ParentTaskId)
                .HasMaxLength(50)
                .HasColumnName("ParentTaskId");

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("Title");

            builder.Property(t => t.Description)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("Description");

            builder.Property(t=>t.Status)
                .IsRequired()
                .HasColumnName("Status");

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasColumnName("Priority");

            builder.Property(t => t.Progress)
                .HasColumnName("Progress");

            builder.Property(t => t.StartAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("StartAt");

            builder.Property(t => t.DueAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("DueAt");

            builder.Property(t => t.CompletedAt)
               .HasColumnType("timestamp with time zone")
               .HasColumnName("CompletedAt");

            builder.Property(t => t.IsDeleted)
               .HasColumnName("IsDeleted");

            builder.Property(t => t.DeletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("DeletedAt");

            builder.Property(t => t.CreatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("CreatedAt");

            builder.Property(t => t.UpdatedAt)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("UpdatedAt");
            
            builder.HasOne(t=>t.ParentTask)
                .WithMany(t=>t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.TaskList)
                   .WithMany(t => t.Items)
                   .HasForeignKey(t => t.ListId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(item => item.Owner)
                   .WithMany(user => user.TaskItems)
                   .HasForeignKey(item => item.OwnerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
