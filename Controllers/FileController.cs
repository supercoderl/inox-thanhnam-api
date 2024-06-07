using ExcelDataReader;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.File;
using InoxThanhNamServer.Models;
using InoxThanhNamServer.Services.DiscountSer;
using InoxThanhNamServer.Services.FileSer;
using InoxThanhNamServer.Services.OrderSer;
using InoxThanhNamServer.Services.ProductSer;
using InoxThanhNamServer.Services.UserSer;
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
        private readonly IProductService _productService;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly IDiscountService _discountService;
        private readonly IOrderService _orderService;

        public FileController(InoxEcommerceContext context, 
            IProductService productService, IFileService fileService, 
            IUserService userService, IDiscountService discountService,
            IOrderService orderService)
        {
            _context = context;
            _productService = productService;
            _fileService = fileService;
            _userService = userService;
            _discountService = discountService;
            _orderService = orderService;
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

        [HttpPost("export-excel")]
        public async Task<IActionResult> ExportExcel(string type)
        {
            switch (type)
            {
                case "product":
                    var products = await _productService.GetProducts(null);
                    if (products.Data != null)
                    {
                        var file = _fileService.CreateFile(products.Data);
                        var excel64 = File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{type}s.xlsx");
                        return StatusCode(200, file);
                    }
                    break;
                case "order":
                    var orders = await _orderService.GetOrders(null);
                    if (orders.Data != null)
                    {
                        var file = _fileService.CreateFile(orders.Data);
                        return StatusCode(200, file);
                    }
                    break;
                case "discount":
                    var discounts = await _discountService.GetDiscounts(null);
                    if (discounts.Data != null)
                    {
                        var file = _fileService.CreateFile(discounts.Data);
                        return StatusCode(200, file);
                    }
                    break;
                case "user":
                    var users = await _userService.GetUsers(null);
                    if (users.Data != null)
                    {
                        var file = _fileService.CreateFile(users.Data);
                        return StatusCode(200, file);
                    }
                    break;
            }

            return StatusCode(400);
        }
    }
}
