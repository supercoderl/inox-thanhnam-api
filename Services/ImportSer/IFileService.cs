using InoxThanhNamServer.Datas;

namespace InoxThanhNamServer.Services.ImportSer
{
    public interface IFileService
    {
        Task<ApiResponse<object>> ImportFile(IFormFile file);
    }
}
