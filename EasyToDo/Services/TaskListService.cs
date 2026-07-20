using EasyToDo.Models;
using EasyToDo.Models.DAO;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services.Database;
using EasyToDo.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services
{
    public sealed class TaskListService(EasyToDoDbContext repository)
    {
        public async Task<ApiResponse<TaskListDetailResponse>> GetTaskListAsync(string id, Guid userId)
        {
            if (!Guid.TryParse(id, out var taskListId))
            {
                return ApiResponseFactory.Failure<TaskListDetailResponse>("Invalid TaskList ID format.");
            }

            var taskList = await repository.TaskLists
                .Include(list => list.Items)
                .AsNoTracking()
                .SingleOrDefaultAsync(list => list.Id == taskListId && list.OwnerId == userId);
            return taskList == null
                ? ApiResponseFactory.Failure<TaskListDetailResponse>("TaskList not found.")
                : ApiResponseFactory.Success(ToDetailResponse(taskList), "TaskList retrieved successfully.");
        }

        public async Task<ApiResponse<List<TaskListResponse>>> CreateTaskListAsync(TaskListCreateRequest request, Guid userId)
        {
            repository.TaskLists.Add(new TaskListDAO
            {
                Color = request.Color,
                Name = request.Name,
                OwnerId = userId
            });
            await repository.SaveChangesAsync();
            return ApiResponseFactory.Success(await GetTaskListResponsesAsync(userId), "TaskList Create Successful");
        }

        public async Task<ApiResponse<List<TaskListResponse>>> UpdateTaskListAsync(string id, TaskListUpdateRequest request, Guid userId)
        {
            if (!Guid.TryParse(id, out var taskListId))
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("Invalid TaskList ID format.");
            }
            var taskList = await repository.TaskLists.SingleOrDefaultAsync(list => list.Id == taskListId && list.OwnerId == userId);
            if (taskList == null)
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("TaskList not found.");
            }

            taskList.Name = request.Name;
            taskList.Color = request.Color;
            taskList.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return ApiResponseFactory.Success(await GetTaskListResponsesAsync(userId), "TaskList Update Successful");
        }

        public async Task<ApiResponse<List<TaskListResponse>>> MarkDeleteTaskListAsync(string id, Guid userId)
        {
            if (!Guid.TryParse(id, out var taskListId))
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("Invalid TaskList ID format.");
            }
            var taskList = await repository.TaskLists.SingleOrDefaultAsync(list => list.Id == taskListId && list.OwnerId == userId);
            if (taskList == null)
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("TaskList not found.");
            }

            taskList.DeletedAt = DateTime.UtcNow;
            taskList.IsDeleted = true;
            taskList.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return ApiResponseFactory.Success(await GetTaskListResponsesAsync(userId), "TaskList MarkDelete Successful");
        }

        public async Task<ApiResponse<List<TaskListResponse>>> UnmarkDeleteTaskListAsync(string id, Guid userId)
        {
            if (!Guid.TryParse(id, out var taskListId))
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("Invalid TaskList ID format.");
            }
            var taskList = await repository.TaskLists.IgnoreQueryFilters().SingleOrDefaultAsync(list => list.Id == taskListId && list.OwnerId == userId);
            if (taskList == null)
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("TaskList not found.");
            }

            taskList.DeletedAt = null;
            taskList.IsDeleted = false;
            taskList.UpdatedAt = DateTime.UtcNow;
            await repository.SaveChangesAsync();
            return ApiResponseFactory.Success(await GetTaskListResponsesAsync(userId), "TaskList Unmark Delete Successful");
        }

        public async Task<ApiResponse<List<TaskListResponse>>> DeleteTaskListAsync(string id, Guid userId)
        {
            if (!Guid.TryParse(id, out var taskListId))
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("Invalid TaskList ID format.");
            }
            var taskList = await repository.TaskLists.IgnoreQueryFilters().SingleOrDefaultAsync(list => list.Id == taskListId && list.OwnerId == userId);
            if (taskList == null)
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("TaskList not found.");
            }
            if (!taskList.IsDeleted)
            {
                return ApiResponseFactory.Failure<List<TaskListResponse>>("TaskList hasn't been marked as deleted.");
            }

            repository.TaskLists.Remove(taskList);
            await repository.SaveChangesAsync();
            return ApiResponseFactory.Success(await GetTaskListResponsesAsync(userId), "TaskList Delete Successful");
        }

        public async Task<ApiResponse<List<TaskListResponse>>> GetDeletedTaskListAsync(Guid userId)
        {
            var taskLists = await repository.TaskLists
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(list => list.OwnerId == userId && list.IsDeleted)
                .ToListAsync();
            return ApiResponseFactory.Success(taskLists.Select(ToResponse).ToList(), "Deleted TaskLists retrieved successfully.");
        }

        private async Task<List<TaskListResponse>> GetTaskListResponsesAsync(Guid userId)
        {
            var taskLists = await repository.TaskLists
                .AsNoTracking()
                .Where(list => list.OwnerId == userId)
                .ToListAsync();
            return taskLists.Select(ToResponse).ToList();
        }

        private static TaskListResponse ToResponse(TaskListDAO taskList) => new(taskList.Id.ToString(), taskList.Name, taskList.Color);

        private static TaskItemResponse ToResponse(TaskItemDAO task) => new(task.Id.ToString(), task.Title, task.Status, task.Priority, task.Progress, task.StartAt, task.DueAt);

        private static TaskListDetailResponse ToDetailResponse(TaskListDAO taskList) => new(
            taskList.Id.ToString(),
            taskList.Name,
            taskList.Color,
            taskList.Items
                .Where(task => task.ParentTaskId == null)
                .OrderBy(task => task.DueAt ?? DateTime.MaxValue)
                .Select(ToResponse)
                .ToList());
    }
}
