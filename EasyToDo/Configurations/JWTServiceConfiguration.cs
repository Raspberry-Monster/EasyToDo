namespace EasyToDo.Configurations
{
    public record JWTServiceConfiguration(string Issuer, string Audience, byte[] SecurityKey, int ExpirationInMinutes);
}
