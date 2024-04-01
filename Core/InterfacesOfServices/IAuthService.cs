using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.InterfacesOfServices
{     
    public interface IAuthService
    {
         //Task<LoginResult> Login(LoginModel model);
        //Task<RegistrationResult> Register(RegisterModel model);

        Task<AuthModel> Register(RegisterModel model);

        //Task<LoginResult> Login(LoginModel model);

        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);

        Task<AuthModel> Login(LoginModel model);

        Task<AuthModel> RefreshToken(string token);

        Task<bool> RevokeToken(string token);


        //Task<LogoutResult> Logout();

    }
}
