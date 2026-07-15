using EasyToDo.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Models.DAO
{
    [EntityTypeConfiguration(typeof(TaskListDAOConfiguration))]
    public class TaskListDAO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required Guid OwnerId { get; set; }
        public required string Color { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public UserDAO Owner { get; set; } = null!;
        public List<TaskItemDAO> Items { get; set; } = [];
    }
}
