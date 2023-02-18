namespace Pokemon.WebAPI.Auth;

public interface ITokenService
{
    public string BuildToken(string key, string issuer, UserDto user);
}