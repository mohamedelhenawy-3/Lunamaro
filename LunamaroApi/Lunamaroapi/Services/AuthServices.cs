using Lunamaroapi.DTOs.userDTO;
using Lunamaroapi.Helper;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Lunamaroapi.Services
{
    public class AuthServices :IAuth
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtTokenGenerator _jwt;



        public AuthServices(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JwtTokenGenerator jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
        }
        public async Task<AuthResponse> RegisterAsync(RegisterRequest req)
        {
            var user = new ApplicationUser { UserName = req.UserName, FullName = req.FulName, Email = req.Email };
            var res = new AuthResponse();


            var result = await _userManager.CreateAsync(user, req.Password);
            if(req.Password != req.ComfirmPassword)
            {
                res.Success = false;
                res.Errors = new List<string> { "Password and Confirm Password do not match." };
                return res;
            }
            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }
            var token = await _jwt.GenerateToken(user);
            return new AuthResponse { Success = true };


        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return new AuthResponse { Success = false, Errors = new List<string>() { "Invalid Email OR PassWord" } };

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded) return new AuthResponse { Success = false, Errors = new List<string>() { "Invalid Email OR PassWord" } };


            var token = await _jwt.GenerateToken(user);
            return new AuthResponse { Success = true, Token = token };
        }



    }
}
