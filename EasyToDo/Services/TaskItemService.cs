using System.Linq.Expressions;
using EasyToDo.Models;
using EasyToDo.Models.DAO;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Models.Enums;
using EasyToDo.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services
{
    public sealed class TaskItemService(EasyToDoDbContext repository)
    {
        public async Task<ApiResponse<List<TaskItemResponse>>> GetTaskItemsAsync(Guid userId)
        {
            var tasks = await GetTaskResponsesAsync(userId, task => task.ParentTaskId == null);
            return new ApiResponse<List<TaskItemResponse>>() { Data = tasks, Message = "Tasks retrieved successfully.", Success = true };
        }

        public async Task<ApiResponse<TaskItemDetailResponse>> GetTaskAsync(string id, Guid userId)
        {
            var task = await FindTaskAsync(id, userId, parentTaskId: false);
            return task == null
                ? Failure<TaskItemDetailResponse>("Task not found.")
                : Success(ToDetailResponse(task), "Task retrieved successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> CreateTaskAsync(TaskItemCreateRequest request, Guid userId)
        {
            if (!Guid.TryParse(request.ListId, out var listId))
            {
                return Failure<List<TaskItemResponse>>("Invalid TaskList ID format.");
            }

            if (!await repository.TaskLists.AnyAsync(list => list.Id == listId && list.OwnerId == userId))
            {
                return Failure<List<TaskItemResponse>>("TaskList not found.");
            }

            repository.TaskItems.Add(new TaskItemDAO
            {
                ListId = listId,
                OwnerId = userId,
                Title = request.Title,
                Description = request.Description,
                StartAt = request.StartAt,
                DueAt = request.DueAt
            });
            await repository.SaveChangesAsync();

            var tasks = await GetTaskResponsesAsync(userId, task => task.ParentTaskId == null);
            return Success(tasks, "Task created successfully.");
        }

        public async Task<ApiResponse<TaskItemDetailResponse>> UpdateTaskAsync(string id, TaskItemUpdateRequest request, Guid userId)
        {
            var task = await FindTaskAsync(id, userId, parentTaskId: false);
            if (task == null)
            {
                return Failure<TaskItemDetailResponse>("Task not found.");
            }

            return await UpdateTaskItemAsync(task, request, userId);
        }

        public async Task<ApiResponse<TaskItemDetailResponse>> UpdateTaskStatusAsync(string id, TaskItemStatusUpdateRequest request, Guid userId, bool isSubTask)
        {
            var task = await FindTaskAsync(id, userId, isSubTask ? true : null);
            if (task == null)
            {
                return Failure<TaskItemDetailResponse>(isSubTask ? "Subtask not found." : "Task not found.");
            }

            task.Status = request.Status;
            task.CompletedAt = request.Status == TaskItemStatus.Completed ? DateTime.UtcNow : null;
            if (request.Status == TaskItemStatus.Completed)
            {
                task.Progress = 100;
            }
            task.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return Success(ToDetailResponse(task), "Task status updated successfully.");
        }

        public async Task<ApiResponse<TaskItemDetailResponse>> UpdateTaskProgressAsync(string id, TaskItemProgressUpdateRequest request, Guid userId)
        {
            if (request.Progress is < 0 or > 100)
            {
                return Failure<TaskItemDetailResponse>("Progress must be between 0 and 100.");
            }

            var task = await FindTaskAsync(id, userId, parentTaskId: false);
            if (task == null)
            {
                return Failure<TaskItemDetailResponse>("Task not found.");
            }

            task.Progress = request.Progress;
            task.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return Success(ToDetailResponse(task), "Task progress updated successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> MarkDeleteTaskAsync(string id, Guid userId)
        {
            var task = await FindTaskAsync(id, userId, parentTaskId: false);
            if (task == null)
            {
                return Failure<List<TaskItemResponse>>("Task not found.");
            }

            await SetDeletedStateAsync(task.Id, userId, true);
            var tasks = await GetTaskResponsesAsync(userId, item => item.ParentTaskId == null);
            return Success(tasks, "Task moved to recycle bin successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> UnmarkDeleteTaskAsync(string id, Guid userId)
        {
            var task = await FindTaskAsync(id, userId, parentTaskId: false, ignoreQueryFilters: true);
            if (task == null || !task.IsDeleted)
            {
                return Failure<List<TaskItemResponse>>("Deleted task not found.");
            }

            await SetDeletedStateAsync(task.Id, userId, false);
            var tasks = await GetTaskResponsesAsync(userId, item => item.ParentTaskId == null);
            return Success(tasks, "Task restored successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> DeleteTaskAsync(string id, Guid userId)
        {
            var task = await FindTaskAsync(id, userId, parentTaskId: false, ignoreQueryFilters: true);
            if (task == null)
            {
                return Failure<List<TaskItemResponse>>("Task not found.");
            }
            if (!task.IsDeleted)
            {
                return Failure<List<TaskItemResponse>>("Task hasn't been marked as deleted.");
            }

            repository.TaskItems.Remove(task);
            await repository.SaveChangesAsync();
            var tasks = await GetTaskResponsesAsync(userId, item => item.ParentTaskId == null);
            return Success(tasks, "Task deleted successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> GetCompletedTasksAsync(Guid userId)
        {
            var tasks = await GetTaskResponsesAsync(userId, task => task.ParentTaskId == null && task.Status == TaskItemStatus.Completed);
            return Success(tasks, "Completed tasks retrieved successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> GetDeletedTasksAsync(Guid userId)
        {
            var tasks = await repository.TaskItems
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(task => task.OwnerId == userId && task.ParentTaskId == null && task.IsDeleted)
                .OrderByDescending(task => task.DeletedAt)
                .Select(task => ToResponse(task))
                .ToListAsync();
            return Success(tasks, "Deleted tasks retrieved successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> GetCalendarTasksAsync(DateTime startAt, DateTime endAt, Guid userId)
        {
            if (startAt > endAt)
            {
                return Failure<List<TaskItemResponse>>("The start date must be earlier than or equal to the end date.");
            }

            var tasks = await GetTaskResponsesAsync(userId, task =>
                task.ParentTaskId == null &&
                ((task.StartAt != null && task.StartAt <= endAt && (task.DueAt == null || task.DueAt >= startAt)) ||
                 (task.StartAt == null && task.DueAt != null && task.DueAt >= startAt && task.DueAt <= endAt)));
            return Success(tasks, "Calendar tasks retrieved successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> GetSubTasksAsync(string taskId, Guid userId)
        {
            if (!Guid.TryParse(taskId, out var parentTaskId))
            {
                return Failure<List<TaskItemResponse>>("Invalid task ID format.");
            }
            if (!await repository.TaskItems.AnyAsync(task => task.Id == parentTaskId && task.OwnerId == userId && task.ParentTaskId == null))
            {
                return Failure<List<TaskItemResponse>>("Task not found.");
            }

            var subTasks = await GetTaskResponsesAsync(userId, task => task.ParentTaskId == parentTaskId);
            return Success(subTasks, "Subtasks retrieved successfully.");
        }

        public async Task<ApiResponse<List<TaskItemResponse>>> CreateSubTaskAsync(string taskId, SubTaskCreateRequest request, Guid userId)
        {
            if (!Guid.TryParse(taskId, out var parentTaskId))
            {
                return Failure<List<TaskItemResponse>>("Invalid task ID format.");
            }
            var parentTask = await repository.TaskItems.SingleOrDefaultAsync(task => task.Id == parentTaskId && task.OwnerId == userId && task.ParentTaskId == null);
            if (parentTask == null)
            {
                return Failure<List<TaskItemResponse>>("Task not found.");
            }

            repository.TaskItems.Add(new TaskItemDAO
            {
                ListId = parentTask.ListId,
                OwnerId = userId,
                ParentTaskId = parentTask.Id,
                Title = request.Title,
                Description = request.Description,
                StartAt = request.StartAt,
                DueAt = request.DueAt
            });
            await repository.SaveChangesAsync();
            var subTasks = await GetTaskResponsesAsync(userId, task => task.ParentTaskId == parentTaskId);
            return Success(subTasks, "Subtask created successfully.");
        }

        public async Task<ApiResponse<TaskItemDetailResponse>> GetSubTaskAsync(string id, Guid userId)
        {
            var subTask = await FindTaskAsync(id, userId, parentTaskId: true);
            return subTask == null
                ? Failure<TaskItemDetailResponse>("Subtask not found.")
                : Success(ToDetailResponse(subTask), "Subtask retrieved successfully.");
        }

        public async Task<ApiResponse<TaskItemDetailResponse>> UpdateSubTaskAsync(string id, TaskItemUpdateRequest request, Guid userId)
        {
            var subTask = await FindTaskAsync(id, userId, parentTaskId: true);
            if (subTask == null)
            {
                return Failure<TaskItemDetailResponse>("Subtask not found.");
            }

            return await UpdateTaskItemAsync(subTask, request, userId);
        }

        public async Task<ApiResponse<object>> DeleteSubTaskAsync(string id, Guid userId)
        {
            var subTask = await FindTaskAsync(id, userId, parentTaskId: true);
            if (subTask == null)
            {
                return Failure<object>("Subtask not found.");
            }

            repository.TaskItems.Remove(subTask);
            await repository.SaveChangesAsync();
            return Success<object>(null, "Subtask deleted successfully.");
        }

        private async Task<ApiResponse<TaskItemDetailResponse>> UpdateTaskItemAsync(TaskItemDAO task, TaskItemUpdateRequest request, Guid userId)
        {
            if (!Guid.TryParse(request.ListId, out var listId))
            {
                return Failure<TaskItemDetailResponse>("Invalid TaskList ID format.");
            }
            if (!await repository.TaskLists.AnyAsync(list => list.Id == listId && list.OwnerId == userId))
            {
                return Failure<TaskItemDetailResponse>("TaskList not found.");
            }
            if (request.Progress is < 0 or > 100)
            {
                return Failure<TaskItemDetailResponse>("Progress must be between 0 and 100.");
            }
            if (task.ParentTaskId.HasValue)
            {
                var parentTask = await repository.TaskItems.SingleOrDefaultAsync(item => item.Id == task.ParentTaskId && item.OwnerId == userId);
                if (parentTask == null || parentTask.ListId != listId)
                {
                    return Failure<TaskItemDetailResponse>("A subtask must belong to the same TaskList as its parent task.");
                }
            }

            task.Title = request.Title;
            task.ListId = listId;
            task.Description = request.Description;
            task.Status = request.Status;
            task.Priority = request.Priority;
            task.Progress = request.Progress;
            task.StartAt = request.StartAt;
            task.DueAt = request.DueAt;
            task.CompletedAt = request.Status == TaskItemStatus.Completed ? task.CompletedAt ?? DateTime.UtcNow : null;
            task.UpdatedAt = DateTime.UtcNow;
            if (!task.ParentTaskId.HasValue)
            {
                await repository.TaskItems
                    .Where(item => item.OwnerId == userId && item.ParentTaskId == task.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(item => item.ListId, listId)
                        .SetProperty(item => item.UpdatedAt, task.UpdatedAt));
            }
            await repository.SaveChangesAsync();
            return Success(ToDetailResponse(task), "Task updated successfully.");
        }

        private async Task SetDeletedStateAsync(Guid taskId, Guid userId, bool isDeleted)
        {
            var updatedAt = DateTime.UtcNow;
            DateTime? deletedAt = isDeleted ? updatedAt : null;
            await repository.TaskItems
                .IgnoreQueryFilters()
                .Where(task => task.OwnerId == userId && task.ParentTaskId == taskId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(task => task.IsDeleted, isDeleted)
                    .SetProperty(task => task.DeletedAt, deletedAt)
                    .SetProperty(task => task.UpdatedAt, updatedAt));
        }

        private async Task<TaskItemDAO?> FindTaskAsync(string id, Guid userId, bool? parentTaskId, bool ignoreQueryFilters = false)
        {
            if (!Guid.TryParse(id, out var taskId))
            {
                return null;
            }

            var tasks = ignoreQueryFilters ? repository.TaskItems.IgnoreQueryFilters() : repository.TaskItems;
            return await tasks.SingleOrDefaultAsync(task => task.Id == taskId && task.OwnerId == userId &&
                (!parentTaskId.HasValue || (parentTaskId.Value ? task.ParentTaskId != null : task.ParentTaskId == null)));
        }

        private async Task<List<TaskItemResponse>> GetTaskResponsesAsync(Guid userId, Expression<Func<TaskItemDAO, bool>> predicate)
        {
            return await repository.TaskItems
                .AsNoTracking()
                .Where(task => task.OwnerId == userId)
                .Where(predicate)
                .OrderBy(task => task.DueAt ?? DateTime.MaxValue)
                .ThenByDescending(task => task.CreatedAt)
                .Select(task => ToResponse(task))
                .ToListAsync();
        }

        private static TaskItemResponse ToResponse(TaskItemDAO task) => new(task.Id.ToString(), task.Title, task.Status, task.Priority, task.Progress, task.StartAt, task.DueAt);

        private static TaskItemDetailResponse ToDetailResponse(TaskItemDAO task) => new(task.Id.ToString(), task.ParentTaskId?.ToString(), task.Title, task.Description, task.Status, task.Priority, task.Progress, task.StartAt, task.DueAt, task.CompletedAt);

        private static ApiResponse<T> Success<T>(T? data, string message) where T : class => new() { Data = data, Message = message, Success = true };

        private static ApiResponse<T> Failure<T>(string message) where T : class => new() { Message = message, Success = false };
    }
}
