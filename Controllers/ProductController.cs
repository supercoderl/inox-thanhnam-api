using AutoMapper;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Datas.User;
using InoxThanhNamServer.Services.ProductSer;
using InoxThanhNamServer.Services.UserSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery]FilterProduct? filter)
        {
            var result = await _productService.GetProducts(filter);
            return StatusCode(result.Status, result);
        }

        [HttpPut("update-product/{ProductID}")]
        public async Task<IActionResult> UpdateProduct(int ProductID, UpdateProductRequest request)
        {
            var result = await _productService.UpdateProduct(ProductID, request);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("delete-product/{ProductID}")]
        public async Task<IActionResult> DeleteProduct(int ProductID)
        {
            var result = await _productService.DeleteProduct(ProductID);
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-product-by-id/{ProductID}")]
        public async Task<IActionResult> GetProductByID(int ProductID)
        {
            var result = await _productService.GetProductByID(ProductID);
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request)
        {
            var result = await _productService.CreateProduct(request);
            return StatusCode(result.Status, result);
        }
    }
}
