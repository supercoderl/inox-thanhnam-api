using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Models;
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using System.Net;

namespace InoxThanhNamServer.Services.ImportSer
{
    public class FileService : IFileService
    {
        public async Task<ApiResponse<object>> ImportFile(IFormFile files)
        {
            try
            {
                return new ApiResponse<object>
                {
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Message = "FileService - ImportFile: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
