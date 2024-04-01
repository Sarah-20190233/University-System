using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Core.InterfacesOfServices;
using System.Security.Cryptography;
using NuGet.Common;
using Helpers;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;



namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        private readonly JWT _jwt;


        public AuthService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor
            , IConfiguration configuration,
            IOptions<JWT> jwt)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _jwt = jwt.Value;
        }

        //public async Task<LoginResult> Login(LoginModel model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.Username);
        //    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        //    {
        //        var authClaims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, user.UserName),
        //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        };

        //        var token = GenerateToken(authClaims);

        //        return new LoginResult
        //        {
        //            success = true,
        //            message = "Login successful",
        //            token = token,
        //            RefreshTokenExpiration = token.ValidTo,

        //        };
        //    }

        //    return new LoginResult
        //    {
        //        success = false,
        //        message = "Invalid username or password"
        //    };
        //}



        //private bool IsJwtWithValidSecurityAlgorithm(ClaimsPrincipal principal)
        //{
        //    return principal.Identity is ClaimsIdentity identity &&
        //           identity.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Alg) &&
        //           identity.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Alg)?.Value == SecurityAlgorithms.HmacSha256;
        //}

        //public async Task<RegistrationResult> Register(RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByNameAsync(model.Username);
        //    if (userExists != null)
        //        return RegistrationResult.UserAlreadyExists;

        //    IdentityUser user = new()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Username
        //    };

        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //        return RegistrationResult.RegistrationFailed;

        //    return RegistrationResult.Success;
        //}




        // You need to update this function
        //private JwtSecurityToken GenerateToken(List<Claim> authClaims)
        //{
        //    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["JWT:ValidIssuer"],
        //        audience: _configuration["JWT:ValidAudience"],
        //        expires: DateTime.Now.AddSeconds(3),
        //        claims: authClaims,
        //        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        //    );



        //    return token;
        //}




        //private AspNetUser GenerateRefreshToken()
        //{
        //    var randomNumber = new byte[32];

        //    using var generator = new RNGCryptoServiceProvider();

        //    generator.GetBytes(randomNumber);

        //    return new AspNetUser
        //    {
        //        Token = Convert.ToBase64String(randomNumber),
        //        ExpiresOn = DateTime.UtcNow.AddDays(10),
        //        CreatedOn = DateTime.UtcNow

        //    };
        //}

        //public async Task<LogoutResult> Logout()
        //{
        //    try
        //    {
        //        // Clear the user's authentication cookies

        //        await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        //        await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        //        return new LogoutResult
        //        {
        //            success = true,
        //            message = "Logout successful"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new LogoutResult
        //        {
        //            success = false,
        //            message = $"Error: {ex.Message}"
        //        };
        //    }
        //}

        public async Task<AuthModel> Register(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                return new AuthModel { Message = "Email is already registered!" };
            }

            if (await _userManager.FindByEmailAsync(model.UserName) is not null)
            {
                return new AuthModel { Message = "UserName is already registered!" };
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };


            // To hash the password
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                string errors = string.Empty;

                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }

                return new AuthModel { Message = errors };

            }

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
               // ExpiresOn = jwtSecurityToken.ValidTo,
                isAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName

            };

        }




        public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }



        public async Task<AuthModel> Login(LoginModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                authModel.Message = "Email is incorrect";
                return authModel;
            }
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Password is incorrect";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            authModel.isAuthenticated = true;
            authModel.Message = "Login Successfull";
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            //authModel.ExpiresOn = jwtSecurityToken.ValidTo;

            if(user.RefreshTokens.Any(t => t.IsActive)) 
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }

            else
            {
                var refreshToken = GetRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);  
            }

            return authModel;
        }

        public async Task<bool> RevokeToken(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                return false;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                return false;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;
   
            await _userManager.UpdateAsync(user);

            return true;
        }
        private RefreshToken GetRefreshToken()
        {
            var randomNumber = new Byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };

        }

        public async Task<AuthModel> RefreshToken(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                authModel.isAuthenticated = false;
                authModel.Message = "Invalid token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t =>  t.Token == token);

            if(!refreshToken.IsActive)
            {
                authModel.isAuthenticated = false;
                authModel.Message = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GetRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.isAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.UserName = user.UserName;
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }

      
    }


}


