using EasyToDo.Models;
using EasyToDo.Models.DAO;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services.Database;
using Microsoft.EntityFrameworkCore;

namespace EasyToDo.Services
{
    public sealed class TaskListService(EasyToDoDbContext repository)
    {
        public async Task<ApiResponse<List<TaskListResponse>>> CreateTaskListAsync(TaskListCreateRequest request, Guid userId)
        {
            var taskList = new TaskListDAO()
            {
                Color = request.Color,
                Name = request.Name,
                OwnerId = userId
            };
            repository.TaskLists.Add(taskList);
            await repository.SaveChangesAsync();
            var taskLists = await repository.TaskLists
                .AsNoTracking()
                .Where(t=>t.OwnerId == userId)
                .Select(list => new TaskListResponse(list.Id.ToString(), list.Name, list.Color))
                .ToListAsync();
            return new ApiResponse<List<TaskListResponse>>() { Data = taskLists, Message = "TaskList Create Successful", Success = true };
        }

        public async Task<ApiResponse<List<TaskListResponse>>> UpdateTaskListAsync(TaskListUpdateRequest request, Guid userId)
        {
            var guidParseResult = Guid.TryParse(request.Id, out Guid taskListId);
            if (!guidParseResult)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "Invalid TaskList ID format." };
            }
            var taskList = await repository.TaskLists.SingleOrDefaultAsync(t => t.Id == taskListId && t.OwnerId == userId);
            if (taskList == null)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "TaskList not found." };
            }
            taskList.Name = request.Name;
            taskList.Color = request.Color;
            await repository.SaveChangesAsync();
            var taskLists = await repository.TaskLists
                .AsNoTracking()
                .Where(t => t.OwnerId == userId)
                .Select(list => new TaskListResponse(list.Id.ToString(), list.Name, list.Color))
                .ToListAsync();
            return new ApiResponse<List<TaskListResponse>>() { Data = taskLists, Message = "TaskList Update Successful", Success = true };
        }

        public async Task<ApiResponse<List<TaskListResponse>>> MarkDeleteTaskListAsync(TaskListDeleteRequest request, Guid userId)
        {
            var guidParseResult = Guid.TryParse(request.Id, out Guid taskListId);
            if (!guidParseResult)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "Invalid TaskList ID format." };
            }
            var taskList = await repository.TaskLists.SingleOrDefaultAsync(t => t.Id == taskListId && t.OwnerId == userId);
            if (taskList == null)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "TaskList not found." };
            }
            taskList.DeletedAt = DateTime.UtcNow;
            taskList.IsDeleted = true;
            await repository.SaveChangesAsync();
            var taskLists = await repository.TaskLists
                .AsNoTracking()
                .Where(t => t.OwnerId == userId && !t.IsDeleted)
                .Select(list => new TaskListResponse(list.Id.ToString(), list.Name, list.Color))
                .ToListAsync();
            return new ApiResponse<List<TaskListResponse>>() { Data = taskLists, Message = "TaskList MarkDelete Successful", Success = true };
        }

        public async Task<ApiResponse<List<TaskListResponse>>> UnmarkDeleteTaskListAsync(TaskListDeleteRequest request, Guid userId)
        {
            var guidParseResult = Guid.TryParse(request.Id, out Guid taskListId);
            if (!guidParseResult)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "Invalid TaskList ID format." };
            }
            var taskList = await repository.TaskLists.IgnoreQueryFilters().SingleOrDefaultAsync(t => t.Id == taskListId && t.OwnerId == userId);
            if (taskList == null)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "TaskList not found." };
            }
            taskList.DeletedAt = null;
            taskList.IsDeleted = false;
            await repository.SaveChangesAsync();
            var taskLists = await repository.TaskLists
                .AsNoTracking()
                .Where(t => t.OwnerId == userId && !t.IsDeleted)
                .Select(list => new TaskListResponse(list.Id.ToString(), list.Name, list.Color))
                .ToListAsync();
            return new ApiResponse<List<TaskListResponse>>() { Data = taskLists, Message = "TaskList Unmark Delete Successful", Success = true };
        }

        public async Task<ApiResponse<List<TaskListResponse>>> DeleteTaskListAsync(TaskListDeleteRequest request, Guid userId)
        {
            var guidParseResult = Guid.TryParse(request.Id, out Guid taskListId);
            if(!guidParseResult)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "Invalid TaskList ID format." };
            }
            var taskList = await repository.TaskLists.IgnoreQueryFilters().SingleOrDefaultAsync(t => t.Id == taskListId && t.OwnerId == userId);
            if (taskList == null)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "TaskList not found." };
            }
            if (!taskList.IsDeleted)
            {
                return new ApiResponse<List<TaskListResponse>>() { Success = false, Message = "TaskList hasn't been marked as deleted." };
            }
            repository.TaskLists.Remove(taskList);
            await repository.SaveChangesAsync();
            var taskLists = await repository.TaskLists
                .AsNoTracking()
                .Where(t => t.OwnerId == userId && !t.IsDeleted)
                .Select(list => new TaskListResponse(list.Id.ToString(), list.Name, list.Color))
                .ToListAsync();
            return new ApiResponse<List<TaskListResponse>>() { Data = taskLists, Message = "TaskList Delete Successful", Success = true };
        }
    }
}
