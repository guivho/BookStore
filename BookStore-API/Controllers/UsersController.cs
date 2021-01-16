using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BookStore_API.DTOs;
using BookStore_API.Contracts;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;
        public UsersController(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILoggerService logger,
            IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Endpoint to register a new user
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [Route("register")] // differentiate between login and register posts
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            try
            {
                var emailAddress = userDTO.EmailAddress;
                var password = userDTO.Password;
                var user = new IdentityUser
                {
                    Email = emailAddress,
                    UserName = emailAddress
                };
                var result = await _userManager.CreateAsync(user, password);
                Info($"User({emailAddress}), pw({password})");
                if (!result.Succeeded)
                {
                    var msg = $"Failed to register \"{emailAddress}\" with pw \"{password}\": ";
                    foreach (var error in result.Errors)
                    {
                        msg += $" {error.Description};";
                    }
                    return InternalError(msg);
                }
                Info("Succeeded");
                return Ok(new { result.Succeeded });
            }
            catch (System.Exception e)
            {
                return InternalError(e);
            }
        }

        /// <summary>
        /// User Login Endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [Route("login")] // differentiate between login and register posts
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            try
            {
                var emailAddress = userDTO.EmailAddress;
                var password = userDTO.Password;
                var result = await _signInManager.PasswordSignInAsync(emailAddress, password, false, false);
                Info($"User({emailAddress}), pw({password})");
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(emailAddress);
                    var tokenString = await GenerateJSONWebToken(user);
                    Info("succeeded");
                    return Ok(new { token = tokenString});
                }
                Warn("Unauthorized");
                return Unauthorized(userDTO);
            }
            catch (System.Exception e)
            {
                return InternalError(e);
            }
        }

        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private void Error(string message)
        {
            _logger.LogError($"{By()}: {message}");
        }
        private void Info(string message)
        {
            _logger.LogInfo($"{By()}: {message}");
        }
        private void Warn(string message)
        {
            _logger.LogWarn($"{By()}: {message}");
        }
        private string By()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller} - {action}";
        }
        private ObjectResult InternalError(System.Exception e)
        {
            Error($"{By()}: {e.Message} - {e.InnerException}");
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }
        private ObjectResult InternalError(string message)
        {
            Error(message);
            return StatusCode(500, message);
        }
    }

}

