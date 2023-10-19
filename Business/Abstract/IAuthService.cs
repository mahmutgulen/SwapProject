using Business.Contants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Entities.Concrete.Dtos.User;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<User> Register(UserRegisterDto userRegisterDto, string password);
        IDataResult<User> Login(UserLoginDto userLoginDto);

        IResult UserExists(string email);
        IDataResult<AccessToken> CreateAccessToken(User user);
        IResult PasswordChange(string token, string newPassword, string confirmPassword);
    }
}
