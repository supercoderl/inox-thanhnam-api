using AutoMapper;
using InoxThanhNamServer.Datas.ProductImage;
using InoxThanhNamServer.Models;
using InoxThanhNamServer.Services.ProductImageSer;
using InoxThanhNamServer.Services.ProductSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _productImageService;
        private readonly IMapper _mapper;

        public ProductImageController(IProductImageService productImageService, IMapper mapper)
        {
            _productImageService = productImageService;
            _mapper = mapper;
        }

        [HttpGet("images/{ProductID}")]
        public async Task<IActionResult> GetImagesByProduct(int ProductID)
        {
            var result = await _productImageService.GetImagesByProduct(ProductID);
            return StatusCode(result.Status, result);
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, CreateProductImageRequest request)
        {
            var result = await _productImageService.GetImagesByProduct(1);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("image/{ImageID}")]
        public async Task<IActionResult> DeleteImage(int ImageID)
        {
            var result = await _productImageService.DeleteImage(ImageID);
            return StatusCode(result.Status, result);
        }
    }
}
