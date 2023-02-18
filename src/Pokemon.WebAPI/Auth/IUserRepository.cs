namespace Pokemon.WebAPI.Auth;

public interface IUserRepository
{
    public UserDto GetUser(UserModel userModel);
}