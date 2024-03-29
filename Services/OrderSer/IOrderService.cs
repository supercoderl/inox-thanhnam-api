using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Order;

namespace InoxThanhNamServer.Services.OrderSer
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderProfile>> CreateOrder(CreateOrderRequest order);
        Task<ApiResponse<OrderItemProfile>> CreateOrderItem(CreateOrderItemRequest orderItem);

        Task<ApiResponse<OrderItemProfile>> UpdateOrderItem(int orderItemID, UpdateOrderItemRequest orderItem);

        Task<ApiResponse<OrderProfile>> UpdateOrder(int? orderID, UpdateOrderRequest order);

        Task<ApiResponse<List<OrderItemProfile>>> GetOrderItemByUser(int orderID);

        Task<ApiResponse<OrderProfile>> GetOrderByUser(Guid userID);

        Task<ApiResponse<OrderProfile>> GetOrderByID(int? orderID);

        Task<ApiResponse<List<OrderProfile>>> GetOrders(string? text, int? status, string? fromDate, string? toDate);

        Task<ApiResponse<Object>> DeleteOrderItem(int orderItemID);
    }
}
