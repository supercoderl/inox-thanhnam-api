using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Authentication;
using InoxThanhNamServer.Models;
using System.Security.Claims;

namespace InoxThanhNamServer.Services.JWT
{
    public interface IJWTService
    {
        LoginResult GenerateToken(User user, List<Claim> claims, DateTime now);
        Task RemoveRefreshToken(string username);
        Task<ApiResponse<LoginResult>> Refresh(string refreshToken, DateTime now);
        Task<bool> RevokeToken(string token);
    }
}
