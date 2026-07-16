using EasyToDo.Models.DAO;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Configurations
{
    public class TaskListDAOConfiguration : IEntityTypeConfiguration<TaskListDAO>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TaskListDAO> builder)
        {
            builder.ToTable("TaskLists");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("Id");

            builder.Property(t => t.OwnerId)
                .IsRequired()
                .HasColumnName("OwnerId");

            builder.Property(t => t.Color)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("Color");

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("Name");

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

            builder.HasOne(list => list.Owner)
                .WithMany(user => user.TaskLists)
                .HasForeignKey(list => list.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(list => list.Items)
                .WithOne(item => item.TaskList)
                .HasForeignKey(item => item.ListId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(t=> !t.IsDeleted);
        }
    }
}
