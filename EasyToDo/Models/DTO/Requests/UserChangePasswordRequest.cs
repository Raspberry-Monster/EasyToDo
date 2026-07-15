namespace EasyToDo.Models.DTO.Requests
{
    public sealed record UserChangePasswordRequest(string OldPassword, string NewPassword);
}
