using ExcelDataReader;
using InoxThanhNamServer.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System.Data;
using System.Net;
using Excel = Microsoft.Office.Interop.Excel;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly InoxEcommerceContext _context;

        public FileController(InoxEcommerceContext context)
        {
            _context = context;
        }
        [Route("ReadFile")]
        [HttpPost]
        public async Task<IActionResult> ReadFile(IFormFile file)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Kiểm tra xem tệp tin có tồn tại không
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                // Đọc dữ liệu từ tệp Excel và xử lý nó
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.First();

                        // Xử lý dữ liệu ở đây...
                        // Ví dụ: Lấy dữ liệu từ cột A
                        int rowCount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowCount; row++)
                        {
                            var cityID = Convert.ToInt32(worksheet.Cells[row, 1].Value?.ToString());
                            var name = worksheet.Cells[row, 2].Value?.ToString();
                            var newDistrict = new District
                            {
                                Name = name,
                                CityID = cityID
                            };
                            await _context.Districts.AddAsync(newDistrict);
                        }
                        await _context.SaveChangesAsync();

                        return StatusCode(200, "Data from Excel has been processed.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
