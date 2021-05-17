using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DigitalAgency.Dal.Storages
{
    public class UserStorage : IUserStorage
    {
        protected readonly UserManager<IdentityUser> UserManager;

        public UserStorage (UserManager<IdentityUser> userManager)
        {
            UserManager = userManager;
        }

        public async Task<bool> IsUserExist(string userPhoneNumber)
        {
            return await UserManager.Users.AnyAsync(u => u.PhoneNumber == userPhoneNumber);
        }

        public async Task<bool> RegisterUser(string userPhoneNumber, string userPassword)
        {
            if (string.IsNullOrEmpty(userPhoneNumber) || string.IsNullOrEmpty(userPassword) || await IsUserExist(userPhoneNumber))
                return false;
            IdentityUser user = new IdentityUser();
            user.UserName = user.PhoneNumber = userPhoneNumber;
            user.PasswordHash = userPassword;
            await UserManager.CreateAsync(user);
            return true;
        }

        public async Task<string> GetToken(string userPhoneNumber, string userPassword)
        {
            if (string.IsNullOrEmpty(userPhoneNumber) || string.IsNullOrEmpty(userPassword))
                return null;
            var user = await UserManager.FindByNameAsync(userPhoneNumber);
            if (user == null || user.PasswordHash != userPassword)
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
