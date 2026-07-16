using EasyToDo.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Models.DAO
{
    [EntityTypeConfiguration(typeof(UserDAOCConfiguration))]
    public class UserDAO
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string NickName { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<TaskListDAO> TaskLists { get; set; } = [];
        public List<TaskItemDAO> TaskItems { get; set; } = [];
    }
}
