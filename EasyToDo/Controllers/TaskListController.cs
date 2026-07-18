using EasyToDo.Models;
using EasyToDo.Models.DTO.Requests;
using EasyToDo.Models.DTO.Responses;
using EasyToDo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EasyToDo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskListController(TaskListService taskListService) : ControllerBase
    {
        [HttpPost("create")]
        [EndpointDescription("任务列表创建")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> CreateTaskListAsync([FromBody] TaskListCreateRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId?.Value))
            {
                return Unauthorized(new ApiResponse<object>() { Success = false, Message = "User ID claim is missing." });
            }
            var guidParseStatus = Guid.TryParse(userId.Value, out Guid parsedUserId);
            if (!guidParseStatus)
            {
                return BadRequest(new ApiResponse<object>() { Success = false, Message = "Invalid User ID format." });
            }
            var result = await taskListService.CreateTaskListAsync(request, parsedUserId);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("update")]
        [EndpointDescription("任务列表更新")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> UpdateTaskListAsync([FromBody] TaskListUpdateRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId?.Value))
            {
                return Unauthorized(new ApiResponse<object>() { Success = false, Message = "User ID claim is missing." });
            }
            var guidParseStatus = Guid.TryParse(userId.Value, out Guid parsedUserId);
            if (guidParseStatus == false)
            {
                return BadRequest(new ApiResponse<object>() { Success = false, Message = "Invalid User ID format." });
            }
            var result = await taskListService.UpdateTaskListAsync(request, parsedUserId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
        [HttpPost("mark-delete")]
        [EndpointDescription("任务列表标记删除")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> MarkDeleteTaskListAsync([FromBody] TaskListDeleteRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId?.Value))
            {
                return Unauthorized(new ApiResponse<object>() { Success = false, Message = "User ID claim is missing." });
            }
            var guidParseStatus = Guid.TryParse(userId.Value, out Guid parsedUserId);
            if (!guidParseStatus)
            {
                return BadRequest(new ApiResponse<object>() { Success = false, Message = "Invalid User ID format." });
            }
            var result = await taskListService.MarkDeleteTaskListAsync(request, parsedUserId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
        [HttpPost("unmark-delete")]
        [EndpointDescription("任务列表取消标记删除")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> UnmarkDeleteTaskListAsync([FromBody] TaskListDeleteRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId?.Value))
            {
                return Unauthorized(new ApiResponse<object>() { Success = false, Message = "User ID claim is missing." });
            }
            var guidParseStatus = Guid.TryParse(userId.Value, out Guid parsedUserId);
            if (!guidParseStatus)
            {
                return BadRequest(new ApiResponse<object>() { Success = false, Message = "Invalid User ID format." });
            }
            var result = await taskListService.UnmarkDeleteTaskListAsync(request, parsedUserId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
        [HttpDelete("delete")]
        [EndpointDescription("任务列表删除 (需先标记删除)")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> DeleteTaskListAsync([FromBody] TaskListDeleteRequest request)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (string.IsNullOrEmpty(userId?.Value))
            {
                return Unauthorized(new ApiResponse<object>() { Success = false, Message = "User ID claim is missing." });
            }
            var guidParseStatus = Guid.TryParse(userId.Value, out Guid parsedUserId);
            if (!guidParseStatus)
            {
                return BadRequest(new ApiResponse<object>() { Success = false, Message = "Invalid User ID format." });
            }
            var result = await taskListService.DeleteTaskListAsync(request, parsedUserId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
