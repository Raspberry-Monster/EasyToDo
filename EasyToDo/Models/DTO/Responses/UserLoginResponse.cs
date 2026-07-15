namespace EasyToDo.Models.DTO.Responses
{
    public class UserLoginResponse
    {
        public string? Token { get; init; }
        public DateTime? Expiration { get; init; }
    }
}
