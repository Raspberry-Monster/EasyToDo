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
        private readonly TaskListService _taskListService = taskListService;

        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<List<TaskListResponse>>>> CreateTaskListAsync([FromBody] TaskListCreateRequest request)
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
            var result = await _taskListService.CreateTaskListAsync(request, parsedUserId);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("update")]
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
            var result = await _taskListService.UpdateTaskListAsync(request, parsedUserId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
