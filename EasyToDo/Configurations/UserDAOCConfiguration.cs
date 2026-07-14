using EasyToDo.Models.DAO;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Configurations
{
    public class UserDAOCConfiguration : IEntityTypeConfiguration<UserDAO>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserDAO> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .HasColumnName("Id");

            builder.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("UserName");

            builder.Property(t => t.NickName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("NickName");

            builder.Property(t => t.PasswordHash)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasColumnName("PasswordHash");

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

            builder.HasMany(user=>user.TaskLists)
                .WithOne(taskList=>taskList.Owner)
                .HasForeignKey(taskList=>taskList.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
