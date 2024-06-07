using InoxThanhNamServer.Datas.ProductReview;
using InoxThanhNamServer.Services.ProductReviewSer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReviewService _productReviewService;

        public ProductReviewController(IProductReviewService productReviewService)
        {
            _productReviewService = productReviewService;
        }

        [HttpGet("reviews/{productID}")]
        public async Task<IActionResult> GetReviews(int productID)
        {
            var result = await _productReviewService.GetReviews(productID);
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-review")]
        public async Task<IActionResult> CreateReview(CreateProductReviewRequest request)
        {
            var result = await _productReviewService.CreateReview(request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("update-review/{productReviewID}")]
        public async Task<IActionResult> UpdateReview(int productReviewID, UpdateProductReviewRequest request)
        {
            var result = await _productReviewService.UpdateReview(productReviewID, request);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("delete-review/{productReviewID}")]
        public async Task<IActionResult> DeleteReview(int productReviewID)
        {
            var result = await _productReviewService.DeleteReview(productReviewID);
            return StatusCode(result.Status, result);
        }
    }
}
