using EasyToDo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace EasyToDo.Utilities
{
    public static class ControllerExtensions
    {
        extension(ControllerBase controller)
        {
            public ActionResult? GetUserId(out Guid userId)
            {
                userId = Guid.Empty;
                var claim = controller.User.Claims.FirstOrDefault(item => item.Type == JwtRegisteredClaimNames.Sub);
                if (string.IsNullOrWhiteSpace(claim?.Value))
                {
                    return controller.Unauthorized(new ApiResponse<object>() { Success = false, Message = "User ID claim is missing." });
                }
                return !Guid.TryParse(claim.Value, out userId)
                    ? controller.BadRequest(new ApiResponse<object>() { Success = false, Message = "Invalid User ID format." })
                    : null;
            }

            public ActionResult<ApiResponse<T>> ToActionResult<T>(ApiResponse<T> result) where T : class => result.Success ? controller.Ok(result) : controller.BadRequest(result);
        }
    }
}
