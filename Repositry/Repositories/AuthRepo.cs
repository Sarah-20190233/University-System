using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AuthRepo : IAuthRepo
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AuthRepo(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(IdentityUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityUser> FindUserByNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
