using InoxThanhNamServer.Datas.Authentication;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.User;

namespace InoxThanhNamServer.Services.UserSer
{
    public interface IUserService
    {
        Task<ApiResponse<LoginResult>> Login(LoginRequest request);
        Task<ApiResponse<UserCreateRequest>> Register(UserCreateRequest newUser);
        Task<ApiResponse<string>> ChangePassword(Guid UserID, ChangePassswordRequest request);
        Task<bool> IsExist(string userName);
        Task<ApiResponse<Object>> Logout(string token);
        Task<ApiResponse<UserProfile>> GetProfile(Guid UserID);
        Task<ApiResponse<List<UserProfile>>> GetUsers();
        Task<ApiResponse<UserUpdateRequest>> UpdateUser(Guid UserID, UserUpdateRequest user);
        Task<ApiResponse<Object>> DeleteUser(Guid UserID);
        ApiResponse<Object> SendBackupEmail(string recipientEmail, string token); 
    }
}
