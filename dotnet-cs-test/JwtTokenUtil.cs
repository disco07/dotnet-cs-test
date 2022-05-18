using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using quest_web.Models;

namespace quest_web.Utils
{
    public class JwtTokenUtil
    {
        public static readonly long JwtTokenValidity = 5 * 360;
        private static readonly string Secret = "etna_quest_jwt_secret_key";

        public static readonly TokenValidationParameters TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, ValidateIssuer = false, ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)), ValidateLifetime = true
        };

        public string GenerateToken(UserDetails user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            return GenerateToken(user.Username, claims);
        }

        public string GenerateToken(string subject, Claim[] claims)
        {
            var jwtToken = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddSeconds(JwtTokenValidity),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                    SecurityAlgorithms.HmacSha512Signature));
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public (ClaimsPrincipal, JwtSecurityToken) DecodeToken(string token)
        {
            var principal =
                new JwtSecurityTokenHandler().ValidateToken(token, TokenValidationParameters, out var validatedToken);
            return (principal, validatedToken as JwtSecurityToken);
        }


        public DateTime GetExpirationDateFromToken(string token)
        {
            var (_, jwt) = DecodeToken(token);
            return jwt.ValidTo;
        }

        public string GetUsernameFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ValidateToken(token, TokenValidationParameters, out var tokenSecure);
            return claims.Identity.Name;
        }


        public bool IsTokenExpired(string token)
        {
            return DateTime.Now < GetExpirationDateFromToken(token);
        }


        public bool ValidateToken(string token, UserDetails details)
        {
            return GetUsernameFromToken(token).Equals(details.Username);
        }
    }
}