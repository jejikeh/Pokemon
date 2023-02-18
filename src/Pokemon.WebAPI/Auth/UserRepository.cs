namespace Pokemon.WebAPI.Auth;

public class UserRepository : IUserRepository
{
    private List<UserDto> _users => new List<UserDto>()
    {
        new UserDto("qwerty", "qwerty"),
        new UserDto("asdfgh", "asdfgh"),
        new UserDto("123", "123"),
    };
    
    public UserDto GetUser(UserModel userModel)
    {
        return _users.FirstOrDefault(u =>
            string.Equals(u.UserName, userModel.UserName) && string.Equals(u.Password, userModel.Password)) ?? throw new Exception();
    }
}