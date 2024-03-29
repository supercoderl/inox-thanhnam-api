using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Email;

namespace InoxThanhNamServer.Services.EmailSer
{
    public interface IEmailService
    {
        Task<ApiResponse<string>> SendEmail(Message message);
    }
}
