using Lunamaroapi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lunamaroapi.Helper
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _config;


        public JwtTokenGenerator(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim (ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                 new Claim(ClaimTypes.Role, "Customer")
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwtSettings:SecretKey"]));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

           var token = new JwtSecurityToken(
           issuer: _config["Jwt:Issuer"],
           audience: _config["Jwt:Audience"],
           claims: claims,
           expires: DateTime.Now.AddMinutes(30),
           signingCredentials: cred
           );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));


        }
    }
}
