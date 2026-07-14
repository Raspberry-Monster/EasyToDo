using EasyToDo.Models.DAO;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services.Database
{
    public sealed class EasyToDoDbContext : DbContext
    {
        public DbSet<UserDAO> Users { get; set; }
        public DbSet<TaskListDAO> TaskLists { get; set; }
        public DbSet<TaskItemDAO> TaskItems { get; set; }

        public EasyToDoDbContext(DbContextOptions<EasyToDoDbContext> options)
        : base(options)
        {
        }
    }
}
