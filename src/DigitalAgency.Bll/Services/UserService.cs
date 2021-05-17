using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DigitalAgency.Bll.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DigitalAgency.Bll.Services
{
    public class UserService : IUserService
    {
        protected readonly UserManager<IdentityUser> _userManager;

        public UserService (UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> IsUserExist(string UserPhoneNumber)
        {
            return await _userManager.Users.AnyAsync(u => u.PhoneNumber == UserPhoneNumber);
        }

        public async Task<bool> RegisterUser(string UserPhoneNumber, string UserPassword)
        {
            if (string.IsNullOrEmpty(UserPhoneNumber) || string.IsNullOrEmpty(UserPassword) || await IsUserExist(UserPhoneNumber))
                return false;
            IdentityUser user = new IdentityUser();
            user.UserName = user.PhoneNumber = UserPhoneNumber;
            user.PasswordHash = UserPassword;
            await _userManager.CreateAsync(user);
            return true;
        }

        public async Task<string> GetToken(string UserPhoneNumber, string UserPassword)
        {
            if (string.IsNullOrEmpty(UserPhoneNumber) || string.IsNullOrEmpty(UserPassword))
                return null;
            var user = await _userManager.FindByNameAsync(UserPhoneNumber);
            if (user == null || user.PasswordHash != UserPassword)
                return null;
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var jwt = new JwtSecurityToken
            (
                issuer: "AutoServiceAuthServer",
                audience: "AutoServiceAuthClient",
                notBefore: DateTime.UtcNow,
                claims: claimsIdentity.Claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(4)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("AutoServiceSecurityKey")), SecurityAlgorithms.HmacSha256));
            var encodedjwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedjwt;
        }
    }
}
