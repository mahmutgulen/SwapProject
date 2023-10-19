using Business.Abstract;
using Entities.Concrete.Dtos.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace SwapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDto userLoginDto)
        {
            var userToLogin = _authService.Login(userLoginDto);
            if (!userToLogin.Success)
            {
                return BadRequest(userToLogin.Message);
            }
            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (result.Success)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto userRegisterDto)
        {
            var userExists = _authService.UserExists(userRegisterDto.UserEmail);
            if (!userExists.Success)
            {
                return BadRequest(userExists.Message);
            }
            var registerResult = _authService.Register(userRegisterDto, userRegisterDto.Password);
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

        [HttpPost("passwordChange")]
        public IActionResult PasswordChange(string token, string newPassword, string confirmPassword)
        {
            var result = _authService.PasswordChange(token, newPassword, confirmPassword);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
        }

    }
}
