using InoxThanhNamServer.Datas;

namespace InoxThanhNamServer.Services.FileSer
{
    public interface IFileService
    {
        Task<ApiResponse<object>> ImportFile(IFormFile file);
        byte[] CreateFile<T>(List<T> source);
        Task<string> UploadFile(IFormFile file, string fileName);
    }
}
