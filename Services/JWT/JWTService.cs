using InoxThanhNamServer.Datas.Authentication;
using InoxThanhNamServer.Datas;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InoxThanhNamServer.Models;

namespace InoxThanhNamServer.Services.JWT
{
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _configuration;
        private readonly InoxEcommerceContext _context;

        public JWTService(IConfiguration configuration, InoxEcommerceContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public LoginResult GenerateToken(User user, List<Claim> claims, DateTime now)
        {
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(
                claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value
            );

            var jwtToken = new JwtSecurityToken(
                _configuration["JWT_Configuration:Issuer"],
                shouldAddAudienceClaim ? _configuration["JWT_Configuration:Audience"] : String.Empty,
                claims,
                expires: now.AddMinutes(Convert.ToDouble(_configuration["JWT_Configuration:TokenExpirationMinutes"])),
                signingCredentials: new SigningCredentials
                (
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_Configuration:SecretKey"]!)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            );

            
            var token = new TokenResult
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpirationMinutes = Convert.ToInt32(_configuration["JWT_Configuration:TokenExpirationMinutes"])
            };

            var refreshToken = new RefreshTokenResult
            {
                RefreshToken = GenerateRefreshToken(),
                ExpirationDays = Convert.ToInt32(_configuration["JWT_Configuration:RefreshTokenExpirationDays"])
            };

            Token newToken = new Token
            {
                RefreshToken = refreshToken.RefreshToken,
                UserID = user.UserID,
                IsActive = true,
            };

            _context.Tokens.Add(newToken);
            _context.SaveChanges();

            return new LoginResult
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }

        public async Task<ApiResponse<LoginResult>> Refresh(string refreshToken, DateTime now)
        {
            var refreshTokenObject = await _context.Tokens.SingleAsync(x => x.RefreshToken == refreshToken);
            var user = await _context.Users.FindAsync(refreshTokenObject.UserID);

            if (refreshTokenObject.RevokeAt != null || !refreshTokenObject.IsActive)
            {
                return new ApiResponse<LoginResult>
                {
                    Success = false,
                    Message = "Token không hợp lệ.",
                    Status = 200
                };
            }

            var roles = (from u in _context.Users
                         join ur in _context.UserRoles
                         on u.UserID equals ur.UserID
                         join r in _context.Roles
                         on ur.RoleID equals r.RoleID
                         where u.UserID == user.UserID
                         select r.Name).ToArray();

            var claims = new List<Claim>
                {
                    new Claim("UserID", user.UserID.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Firstname + " " + user.Lastname),
                };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var result = GenerateToken(user, claims, now);
            return new ApiResponse<LoginResult>
            {
                Success = true,
                Message = "Refresh Token thành công.",
                Data = result,
                Status = 200
            };
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = true,
                ValidAudience = _configuration["JWT_Configuration:Audience"],
                ValidIssuer = _configuration["JWT_Configuration:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_Configuration:SecretKey"]!)),
                ValidateLifetime = true //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public Task RemoveRefreshToken(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RevokeToken(string token)
        {
            var item = await _context.Tokens.FirstOrDefaultAsync(x => x.RefreshToken.Equals(token));
            if (item == null || !item.IsActive || item.RevokeAt != null)
            {
                return false;
            }

            item.IsActive = false;
            item.RevokeAt = DateTime.Now;

            _context.Tokens.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
