using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Projekt.Data.Models;
using Projekt.Data.Repositories;
using Projekt.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IConfiguration _config;
        private readonly IUserRepository userRepository;

        public AuthController(IConfiguration config, IUserRepository userRepository)
        {
            _config = config;
            this.userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserViewModel login)
        {
            IActionResult response;
            var result = await userRepository.AuthenticateUser(new User { UserName = login.UserName}, login.Password);

            if (result.Success)
            {
                var tokenString = GenerateJSONWebToken(result.Data);
                response = Ok(new { token = tokenString, message=result.Message });
            }
            else
            {
                response = Unauthorized(new {message=result.Message});
            }
            return response;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserViewModel login)
        {
            if(ModelState.IsValid)
            {
                var result = await userRepository.CreateUser(new User { UserName=login.UserName}, login.Password);
                var msg = new { message = result.Message };
                if (result.Success)
                {
                    return Ok(msg);
                }
                else
                {
                    return BadRequest(msg);
                }
            }

            return BadRequest(new {message = "Invalid data"});
        }



        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
            new Claim(JwtRegisteredClaimNames.NameId, userInfo.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
