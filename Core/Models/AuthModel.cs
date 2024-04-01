using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AuthModel
    {
        public string Message { get; set; } 

        public bool isAuthenticated { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        //public DateTime ExpiresOn { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }
    }
}
