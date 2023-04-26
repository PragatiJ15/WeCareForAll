using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WeCareForAll.Models.DTO;
using WeCareForAll.Repositories.Abstract;

namespace WeCareForAll.Repositories.Domain
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public ClaimsPrincipal GetClaimsPrincipalFromExpriedToken(string token)
        {
            //getting token validation parameters

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                ValidateLifetime = false,

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            //principal
            var principal = tokenHandler.ValidateToken(token,tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public string GetRefereshtoken()
        {
            //generating a randomnumber as a byte 
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }

        }

        public TokenResponse GetToken(IEnumerable<Claim> claim)
        {
            //accesing siginkey from appsetting.json 
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            //creating a token ad also getting valid users
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: claim,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            string tokenString = new  JwtSecurityTokenHandler().WriteToken(token);
            //returing token string to user
            return new TokenResponse { TokenString= tokenString , ValidTo= token.ValidTo};

        }
    }
}
