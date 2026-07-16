namespace EasyToDo.Models.DTO.Responses
{
    public record UserLoginResponse(string? Token, DateTime? Expiration);
}
