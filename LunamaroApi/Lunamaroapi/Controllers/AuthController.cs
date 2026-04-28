using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Social;
using Lunamaroapi.DTOs.userDTO;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Implements;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IIdentityService _identityService;
        public AuthController(IAuthServices authservices, IIdentityService s)
        {
            _authServices = authservices;
            _identityService = s;
        }

        [HttpPost("social-login")]
        public async Task<IActionResult> SocialLogin([FromBody] SocialLoginRequest request)
        {
            try
            {
                var result = await _identityService.LoginWithSocialAsync(request.Provider, request.Token);

                if (!result.Success) return Unauthorized(new { error = result.Message });

                return Ok(result);
            }
            catch (Exception ex)
            {
                // ex.StackTrace is the most important part here!
                return StatusCode(500, $"Error: {ex.Message} | Line: {ex.StackTrace}");
            }

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
            try
            {
                var result = await _authServices.LoginAsync(loginreq);
                return Ok(result);
            }
            catch (Exception ex)
            {
               
                return BadRequest(new { message = ex.Message });
            }
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
