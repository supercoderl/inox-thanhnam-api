using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using InoxThanhNamServer.Datas;
using NPOI.XSSF.UserModel;
using System.Net;

namespace InoxThanhNamServer.Services.FileSer
{
    public class FileService : IFileService
    {
        private Cloudinary _cloudinary;

        public FileService(IConfiguration configuration)
        {
            _cloudinary = new Cloudinary(new Account
            (
                configuration["AccountSettings:CloudName"],
                configuration["AccountSettings:ApiKey"],
                configuration["AccountSettings:ApiSecret"]
            ));
        }

        public async Task<ApiResponse<object>> ImportFile(IFormFile files)
        {
            try
            {
                await Task.CompletedTask;
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

        public byte[] CreateFile<T>(List<T> source)
        {
            try
            {
                var workbook = new XSSFWorkbook();
                var sheet = workbook.CreateSheet("Sheet1");
                var rowHeader = sheet.CreateRow(0);

                var properties = typeof(T).GetProperties();

                //header
                var font = workbook.CreateFont();
                font.IsBold = true;
                var style = workbook.CreateCellStyle();
                style.SetFont(font);

                var colIndex = 0;
                foreach (var property in properties)
                {
                    var cell = rowHeader.CreateCell(colIndex);
                    cell.SetCellValue(property.Name);
                    cell.CellStyle = style;
                    colIndex++;
                }
                //end header


                //content
                var rowNum = 1;
                foreach (var item in source)
                {
                    var rowContent = sheet.CreateRow(rowNum);

                    var colContentIndex = 0;
                    foreach (var property in properties)
                    {
                        var cellContent = rowContent.CreateCell(colContentIndex);
                        var value = property.GetValue(item, null);

                        if (value == null)
                        {
                            cellContent.SetCellValue("");
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            cellContent.SetCellValue(value.ToString());
                        }
                        else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                        {
                            cellContent.SetCellValue(Convert.ToInt32(value));
                        }
                        else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?))
                        {
                            cellContent.SetCellValue(Convert.ToDouble(value));
                        }
                        else if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        {
                            var dateValue = (DateTime)value;
                            cellContent.SetCellValue(dateValue.ToString("yyyy-MM-dd"));
                        }
                        else cellContent.SetCellValue(value.ToString());

                        colContentIndex++;
                    }

                    rowNum++;
                }

                //end content


                var stream = new MemoryStream();
                workbook.Write(stream);
                var content = stream.ToArray();

                return content;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /*		public async Task<ApiResponse<Object>> ExportFile(byte[] data, string fileName)
                {
                    try
                    {
                        await Task.CompletedTask;
                        FileContentResult file = new Controller(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                    catch (Exception ex)
                    {
                        return new ApiResponse<object>
                        {
                            Success = false,
                            Message = "FileService - Export: " + ex.Message,
                            Status = (int)HttpStatusCode.InternalServerError
                        };
                    }
                //}*/

        public async Task<string> UploadFile(IFormFile file, string fileName)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream)
                    };
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    return uploadResult.Url.ToString();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
