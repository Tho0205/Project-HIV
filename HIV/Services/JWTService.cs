using HIV.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HIV.Services
{
    public class JWTService : IJwtService
    {
        private readonly IConfiguration _config;

        public JWTService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string GenerateToken(string fullName, string role, int userId, int accountId, string user_avatar)
        {
            var jwtsetting = _config.GetSection("JwtSettings");
            var secretkey = jwtsetting["SecretKey"];
            var issuer = jwtsetting["Issuer"];
            var audience = jwtsetting["Audience"];
            var timegae = int.Parse(jwtsetting["ExpirationInMinutes"]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("AccountId", accountId.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role),
                new Claim("UserAvatar", user_avatar),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(timegae),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var jwtsetting = _config.GetSection("JwtSettings");
                var secretkey = jwtsetting["SecretKey"];
                var issuer = jwtsetting["Issuer"];
                var audience = jwtsetting["Audience"];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey));

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    // dùng để ktra chữ kí số trong token, nếu sai secretkey -> token ko hợp lệ
                    ValidateIssuerSigningKey = true,
                    // dùng để xác minh chữ kí
                    IssuerSigningKey = key,

                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    ValidateAudience = true,
                    ValidAudience = audience,

                    // ktra thời gian sống của token
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }catch
            {
                return null;
            }
        }
    }
}
