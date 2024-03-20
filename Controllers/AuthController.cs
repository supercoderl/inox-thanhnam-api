using AutoMapper;
using InoxThanhNamServer.Datas.Authentication;
using InoxThanhNamServer.Datas.User;
using InoxThanhNamServer.Services.JWT;
using InoxThanhNamServer.Services.UserSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJWTService _jwtService;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService, IJWTService jwtService, IMapper mapper)
        {
            _userService = userService;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _userService.Login(request);
            return StatusCode(result.Status, result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateRequest newUser)
        {
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
            var result = await _userService.Register(newUser);
            return StatusCode(result.Status, result);
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePassswordRequest request)
        {
            Guid userID = Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == "UserID")!.Value);
            var result = await _userService.ChangePassword(userID, request);
            return StatusCode(result.Status, result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> RevokeToken(RefreshTokenRequest refreshToken)
        {
            var result = await _userService.Logout(refreshToken.RefreshToken);
            return StatusCode(result.Status, result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _jwtService.Refresh(request.RefreshToken, DateTime.Now);
            return StatusCode(result.Status, result);
        }
    }
}
