using System;
using System.Text;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using API_ElevWiki.Interfaces;
using API_ElevWiki.Models;

namespace API_ElevWiki.Repository
{
    public class TokenService : ITokenService
    {
        private readonly IDbInfoService iDbInfoService;

        private static readonly string filePath = ConfigurationManager.AppSettings["AppSettingsJWT"];

        public TokenService(IDbInfoService iDbInfoService)
        {
            this.iDbInfoService = iDbInfoService;
        }

        #region Token methods
        public async Task<string> GenerateAccessToken(IEnumerable<Claim> claims)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(filePath));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenOptions = new(
                signingCredentials: credentials,
                issuer: "http://172.18.3.153:46465",
                audience: "http://172.18.3.153:46465",
                expires: DateTime.UtcNow.AddMinutes(15),
                claims: claims
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return await Task.FromResult(tokenString);
        }

        public async Task<string> GenerateRandomToken()
        {
            byte[] randomNumber = new byte[32];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                string holder = Convert.ToBase64String(randomNumber);

                // Manipulates the string to avoid escape sequences in the URL.
                string token = holder.Replace(';', '0').Replace(':', '1').Replace('/', '2')
                    .Replace('?', '3').Replace('@', '4').Replace('&', '5').Replace('=', '6')
                    .Replace('+', '7').Replace('$', '8').Replace(',', '9').Replace('(', 'a')
                    .Replace(')', 'b').Replace('-', 'c').Replace('_', 'd').Replace('.', 'e');

                return await Task.FromResult(token);
            }
        }

        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateAudience = true, 
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(filePath)),
                ValidateLifetime = false
            };

            JwtSecurityTokenHandler tokenHandler = new();

            ClaimsPrincipal principal = tokenHandler.ValidateToken
                (token, tokenValidationParameters, out SecurityToken securityToken);

            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals
                (SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return await Task.FromResult(principal);
        }

        public async Task<string> FetchEmailFromClaims(string token)
        {
            var holder = GetPrincipalFromExpiredToken(token).Result;

            string emailAddress = holder.FindFirst("Email").Value;

            return await Task.FromResult(emailAddress);
        }

        public async Task<List<Claim>> AddEmailClaim(string email)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, email)
            };
            return await Task.FromResult(claims);
        }

        public async Task<List<Claim>> AddRoleClaim(string email)
        {
            LoginHolder user = await iDbInfoService.GetLoginHolderFromEmail(email);

            if (user.admin == true)
            {
                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Role, "Admin")
                };
                return await Task.FromResult(claims);
            }
            else
            {
                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Role, "User")
                };
                return await Task.FromResult(claims);
            }
        }
        #endregion
    }
}
