namespace EasyToDo.Models.DTO.Requests
{
    public sealed record UserRegisterRequest(string Username, string Nickname, string Password);
}
