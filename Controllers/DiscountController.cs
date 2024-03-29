using InoxThanhNamServer.Datas.Discount;
using InoxThanhNamServer.Services.DiscountSer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("discounts")]
        public async Task<IActionResult> GetDiscounts()
        {
            var result = await _discountService.GetDiscounts();
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-discount")]
        public async Task<IActionResult> CreateDiscount(CreateDiscountRequest request)
        {
            var result = await _discountService.CreateDiscount(request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("update-discount/{discountID}")]
        public async Task<IActionResult> UpdateDiscount(int discountID, UpdateDiscountRequest request)
        {
            var result = await _discountService.UpdateDiscount(discountID, request);
            return StatusCode(result.Status, result);
        }
    }
}
