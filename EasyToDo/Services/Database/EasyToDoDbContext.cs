using EasyToDo.Models.DAO;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services.Database
{
    public sealed class EasyToDoDbContext(DbContextOptions<EasyToDoDbContext> options) : DbContext(options)
    {
        public DbSet<UserDAO> Users { get; set; }
        public DbSet<TaskListDAO> TaskLists { get; set; }
        public DbSet<TaskItemDAO> TaskItems { get; set; }
    }
}
