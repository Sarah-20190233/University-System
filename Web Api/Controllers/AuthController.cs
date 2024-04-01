using Application;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Core.InterfacesOfServices;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Web_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IConfiguration _configuration;

        private readonly UserManager<IdentityUser> _userManager;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.Register(model);

            if (!result.isAuthenticated)
            {
                return BadRequest(result.Message);
            }


            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);


            return Ok(result);

        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.Login(model);

            if (!result.isAuthenticated)
            {
                return BadRequest(result.Message);
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            return Ok(result);

        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            //   var accessToken = Request.

            var result = await _authService.RefreshToken(refreshToken);
            if (!result.isAuthenticated)
            {
                return BadRequest(result);
            }

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);

        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string token)
        {
            var result = await _authService.RefreshToken(token);

            if (!result.isAuthenticated)
            {
                return BadRequest(result); // Return 400 Bad Request with authModel as response body
            }

            return Ok(result); // Return 200 OK with authModel as response body
        }



        //[HttpGet("refreshToken")]
        //public async Task<IActionResult> RefreshToken()
        //{
        //    var refreshToken = Request.Cookies["refreshToken"];

        //    var result = await _authService.RefreshToken(refreshToken);

        //    if (!result.isAuthenticated)
        //    {
        //        return BadRequest();
        //    }

        //    //Check if the refresh token is expired
        //if (result.RefreshTokenExpiration > DateTime.UtcNow && )
        //    {
        //        // Revoke the expired refresh token
        //        var revokeResult = await _authService.RevokeToken(refreshToken);
        //        if (!revokeResult)
        //        {
        //            return BadRequest("Failed to revoke expired refresh token.");
        //        }
        //    }


        //       SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

        //       return Ok(result);
        //  }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if(string.IsNullOrEmpty (token))
            {
                return BadRequest("Token is required!");
            }

            var result = await _authService.RevokeToken(token);

            if(!result)
            {
                return BadRequest("Token is invalid!");
            }
            
            return Ok("Token has been revoked successfully!");
            
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        //[HttpGet("refreshTokenExpiration")]
        //public IActionResult GetRefreshTokenExpiration([FromQuery] string refreshToken)
        //{
            
        //    var refreshTokenExpiration = refreshToken.ExpiresOn.ToLocalTime();

        //    return Ok(new { refreshTokenExpiration });
        //}

        //[HttpPost]
        //[Route("login")]
        //public async Task<IActionResult> Login([FromBody] LoginModel model)
        //{
        //    var loginResult = await _authService.Login(model);

        //    if (loginResult.success)
        //    {
        //        return Ok(new
        //        {
        //            success = true,
        //            message = loginResult.message,
        //            token = new JwtSecurityTokenHandler().WriteToken(loginResult.token),
        //            expiration = loginResult.RefreshTokenExpiration,


        //        });
        //    }
        //    else
        //    {
        //        return BadRequest(new
        //        {
        //            success = false,
        //            message = loginResult.message
        //        });
        //    }
        //}

        //[HttpPost]
        //[Route("logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    var logoutResult = await _authService.Logout();

        //    if (logoutResult.success)
        //    {
        //        return Ok(new { success = true, message = "Logout successful" });
        //    }
        //    else
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = logoutResult.message });
        //    }
        //}







        //private JwtSecurityToken CreateToken(List<Claim> authClaims)
        //{
        //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        //    _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["JWT:ValidIssuer"],
        //        audience: _configuration["JWT:ValidAudience"],
        //        expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
        //        claims: authClaims,
        //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //        );

        //    return token;
        //}






    }
}
