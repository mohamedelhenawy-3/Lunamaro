using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.userDTO;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Implements;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        public AuthController(IAuthServices authservices)
        {
            _authServices = authservices;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Resgister([FromBody] RegisterRequest registerreq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authServices.RegisterAsync(registerreq);
        

            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginreq)
        {

            var result = await _authServices.LoginAsync(loginreq);
            return Ok(result);
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var result = await _authServices.RefreshTokenAsync(dto.RefreshToken, dto.DeviceId);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto dto)
        {
            var result = await _authServices.LogoutAsync(dto.RefreshToken, dto.DeviceId);
            return Ok(result);
        }
    }
}
