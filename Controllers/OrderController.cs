using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Services.OrderSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPut("update-order/{OrderID}")]
        public async Task<IActionResult> UpdateOrder(int OrderID, UpdateOrderRequest request)
        {
            var result = await _orderService.UpdateOrder(OrderID, request);
            return StatusCode(result.Status, result);
        }

        [HttpPut("update-order-item/{OrderItemID}")]
        public async Task<IActionResult> UpdateOrderItem(int OrderItemID, UpdateOrderItemRequest request)
        {
            var result = await _orderService.UpdateOrderItem(OrderItemID, request);
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-order-item/{OrderID}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(int OrderID)
        {
            var result = await _orderService.GetOrderItemByUser(OrderID);
            return StatusCode(result.Status, result);
        }

        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var result = await _orderService.GetOrders();
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-order-by-user")]
        public async Task<IActionResult> GetOrderByUser()
        {
            string? userID = User.FindFirstValue("UserID");
            if (userID is null)
            {
                return Unauthorized();
            }
            var result = await _orderService.GetOrderByUser(Guid.Parse(userID));
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            string? userID = User.FindFirstValue("UserID");
            if (userID is null)
            {
                return Unauthorized();
            }
            request.UserID = Guid.Parse(userID);
            var result = await _orderService.CreateOrder(request);
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-order-item")]
        public async Task<IActionResult> CreateOrderItem(CreateOrderItemRequest request)
        {
            var result = await _orderService.CreateOrderItem(request);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("delete-order-item/{OrderItemID}")]
        public async Task<IActionResult> DeleteOrderItem(int OrderItemID)
        {
            var result = await _orderService.DeleteOrderItem(OrderItemID);
            return StatusCode(result.Status, result);
        }
    }
}
