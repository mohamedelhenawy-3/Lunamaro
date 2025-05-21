using Lunamaroapi.DTOs.userDTO;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _authService;

        public AuthController(IAuth authservice)
        {
            _authService = authservice;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Resgister([FromBody] RegisterRequest registerreq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(registerreq);
            if (!result.Success) return BadRequest(result);


            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginreq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAsync(loginreq);
            if (!result.Success) return Unauthorized(result);



            return Ok(result);
        }
    }
}
