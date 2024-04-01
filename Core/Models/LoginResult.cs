using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class LoginResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        //public JwtSecurityToken  token { get; set; }

        public string token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }

    }
}
