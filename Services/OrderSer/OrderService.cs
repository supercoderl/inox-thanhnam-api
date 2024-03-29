using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Models;
using InoxThanhNamServer.Services.ProductSer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;

namespace InoxThanhNamServer.Services.OrderSer
{
    public class OrderService : IOrderService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public OrderService(InoxEcommerceContext context, IMapper mapper, IProductService productService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
        }
        public async Task<ApiResponse<OrderProfile>> CreateOrder(CreateOrderRequest order)
        {
            try
            {
                await Task.CompletedTask;
                var orderEntity = _mapper.Map<Order>(order);
                await _context.Orders.AddAsync(orderEntity);
                await _context.SaveChangesAsync();
                return new ApiResponse<OrderProfile>
                {
                    Success = true,
                    Message = "Tạo giỏ hàng thành công.",
                    Data = _mapper.Map<OrderProfile>(orderEntity),
                    Status = (int)HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderProfile>
                {
                    Success = false,
                    Message = "OrderService - CreateOrder: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        private async Task<bool> UpdateTotalAmount(CreateOrderItemRequest? request, UpdateOrderItemRequest? request1, int q)
        {
            try
            {
                await Task.CompletedTask;
                var order = await GetOrderByID(request != null ? request.OrderID : request1.OrderID);
                var product = await _productService.GetProductByID(request != null ? request.ProductID: request1.ProductID);
                if(order.Data == null || product.Data == null)
                {
                    return false;
                }
                var orderEntity = await GetOrderByID(order.Data.OrderID);
                if(orderEntity.Data != null)
                {
                    var updateOrder = new UpdateOrderRequest
                    {
                        OrderID = orderEntity.Data.OrderID,
                        UserID = orderEntity.Data.UserID,
                        OrderDate = DateTime.Now,
                        TotalAmount = order.Data.TotalAmount + q * product.Data.Price,
                        Status = order.Data.Status,
                    };
                    var result = await UpdateOrder(request != null ? request.OrderID : request1.OrderID, updateOrder);
                    return (result.Success && result.Data != null);
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ApiResponse<OrderItemProfile>> CreateOrderItem(CreateOrderItemRequest orderItem)
        {
            try
            {
                await Task.CompletedTask;

                var orderItemExists = await _context.OrderItems.FirstOrDefaultAsync(x => x.ProductID == orderItem.ProductID && x.OrderID == orderItem.OrderID);
                if(orderItemExists != null)
                {
                    var q = orderItemExists.Quantity > orderItem.Quantity ? -1 : 1;
                    var result = await UpdateTotalAmount(orderItem, null, q);
                    if (!result)
                    {
                        return new ApiResponse<OrderItemProfile>
                        {
                            Success = false,
                            Message = "Lỗi cập nhật đơn hàng",
                            Status = (int)HttpStatusCode.OK
                        };
                    }

                    var updateOrderItem = new UpdateOrderItemRequest
                    {
                        OrderItemID = orderItemExists.OrderItemID,
                        OrderID = orderItemExists.OrderID,
                        ProductID = orderItemExists.ProductID,
                        Quantity = orderItemExists.Quantity + q,
                    };

                    _mapper.Map(updateOrderItem, orderItemExists);
                    _context.OrderItems.Update(orderItemExists);
                    await _context.SaveChangesAsync();

                    return new ApiResponse<OrderItemProfile>
                    {
                        Success = true,
                        Message = "Đã cập nhật đơn hàng.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var isUpdatedOrder = await UpdateTotalAmount(orderItem, null, orderItem.Quantity);
                if (!isUpdatedOrder)
                {
                    return new ApiResponse<OrderItemProfile>
                    {
                        Success = false,
                        Message = "Lỗi cập nhật đơn hàng",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var orderItemEntity = _mapper.Map<OrderItem>(orderItem);
                await _context.OrderItems.AddAsync(orderItemEntity);
                await _context.SaveChangesAsync();
                return new ApiResponse<OrderItemProfile>
                {
                    Success = true,
                    Message = "Đã thêm sản phẩm vào giỏ hàng.",
                    Data = _mapper.Map<OrderItemProfile>(orderItemEntity),
                    Status = (int)HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderItemProfile>
                {
                    Success = false,
                    Message = "OrderService - CreateOrderItem: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<Object>> DeleteOrderItem(int orderItemID)
        {
            try
            {
                await Task.CompletedTask;
                var orderItem = await _context.OrderItems.FindAsync(orderItemID);
                if(orderItem == null)
                {
                    return new ApiResponse<Object>
                    {
                        Success = false,
                        Message = "Không tồn tại đơn hàng này.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var isUpdatedOrder = await UpdateTotalAmount(null, _mapper.Map<UpdateOrderItemRequest>(orderItem), -(orderItem.Quantity));
                if (!isUpdatedOrder)
                {
                    return new ApiResponse<Object>
                    {
                        Success = false,
                        Message = "Lỗi cập nhật đơn hàng",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();
                return new ApiResponse<Object>
                {
                    Success = true,
                    Message = "Xóa đơn hàng thành công.",
                    Status = (int)HttpStatusCode.OK
                };
            } 
            catch (Exception ex)
            {
                return new ApiResponse<Object>
                {
                    Success = false,
                    Message = "OrderService - DeleteOrderItem: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<OrderProfile>> GetOrderByUser(Guid userID)
        {
            try
            {
                await Task.CompletedTask;
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.UserID == userID && x.Status == 0);
                if (order == null)
                {
                    var newOrder = new CreateOrderRequest
                    {
                        UserID = userID,
                        TotalAmount = 0,
                        Status = 0,
                    };
                    var result = await CreateOrder(newOrder);
                    return new ApiResponse<OrderProfile>
                    {
                        Success = false,
                        Message = "Tìm thấy giỏ hàng.",
                        Data = result.Data,
                        Status = (int)HttpStatusCode.OK
                    };
                }

                return new ApiResponse<OrderProfile>
                {
                    Success = true,
                    Message = "Tìm thấy giỏ hàng.",
                    Data = _mapper.Map<OrderProfile>(order),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderProfile>
                {
                    Success = false,
                    Message = "OrderService - GetOrderByUser: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<OrderItemProfile>>> GetOrderItemByUser(int orderID)
        {
            try
            {
                await Task.CompletedTask;
                var orderItems = await _context.OrderItems.Where(x => x.OrderID == orderID).ToListAsync();

                if(!orderItems.Any())
                {
                    return new ApiResponse<List<OrderItemProfile>>
                    {
                        Success = false,
                        Message = "Giỏ hàng trống.",
                        Data = new List<OrderItemProfile>(),
                        Status = (int)HttpStatusCode.OK
                    };
                }

                return new ApiResponse<List<OrderItemProfile>>
                {
                    Success = true,
                    Message = "Tìm thấy các đơn hàng.",
                    Data = orderItems.Select(x => _mapper.Map<OrderItemProfile>(x)).ToList(),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<OrderItemProfile>>
                {
                    Success = false,
                    Message = "OrderService - GetOrderItemByUser: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<OrderProfile>>> GetOrders(string? text, int? status, string? fromDate, string? toDate)
        {
            try
            {
                await Task.CompletedTask;
                var orders = await _context.Orders.FromSqlRaw("EXEC sp_get_orders @Status, @FromDate, @ToDate",
                                new SqlParameter("@Status", status ?? (object)DBNull.Value),
                                new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value),
                                new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value)).ToListAsync();
                if(!orders.Any())
                {
                    return new ApiResponse<List<OrderProfile>>
                    {
                        Success = false,
                        Message = "Đơn hàng trống",
                        Status = (int)HttpStatusCode.OK
                    };
                }
                return new ApiResponse<List<OrderProfile>>
                {
                    Success = true,
                    Message = "Tìm thấy đơn hàng",
                    Data = orders.Select(x => _mapper.Map<OrderProfile>(x)).ToList(),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch(Exception ex)
            {
                return new ApiResponse<List<OrderProfile>>
                {
                    Success = false,
                    Message = ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<OrderProfile>> UpdateOrder(int? orderID, UpdateOrderRequest order)
        {
            try
            {
                await Task.CompletedTask;

                if (orderID != order.OrderID)
                {
                    return new ApiResponse<OrderProfile>
                    {
                        Success = false,
                        Message = "Giỏ hàng không đúng.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                var orderEntity = await _context.Orders.FindAsync(orderID);

                if (orderEntity == null)
                {
                    return new ApiResponse<OrderProfile>
                    {
                        Success = false,
                        Message = "Không thể cập nhật vì giỏ hàng không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                _mapper.Map(order, orderEntity);
                _context.Orders.Update(orderEntity);
                await _context.SaveChangesAsync();

                return new ApiResponse<OrderProfile>
                {
                    Success = true,
                    Message = "Cập nhật giỏ hàng thành công.",
                    Data = _mapper.Map<OrderProfile>(orderEntity),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderProfile>
                {
                    Success = false,
                    Message = "OrderService - UpdateOrder: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<OrderItemProfile>> UpdateOrderItem(int orderItemID, UpdateOrderItemRequest orderItem)
        {
            try
            {
                await Task.CompletedTask;

                if (orderItemID != orderItem.OrderItemID)
                {
                    return new ApiResponse<OrderItemProfile>
                    {
                        Success = false,
                        Message = "Đơn hàng không đúng.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                var orderItemEntity = await _context.OrderItems.FindAsync(orderItemID);

                if (orderItemEntity == null)
                {
                    var newOrderItem = new CreateOrderItemRequest
                    {
                        OrderID = orderItem.OrderID,
                        ProductID = orderItem.ProductID,
                        Quantity = orderItem.Quantity,
                        CreatedAt = DateTime.Now
                    };

                    var isUpdatedOrder = await UpdateTotalAmount(newOrderItem, null, orderItem.Quantity);
                    if (!isUpdatedOrder)
                    {
                        return new ApiResponse<OrderItemProfile>
                        {
                            Success = false,
                            Message = "Lỗi cập nhật đơn hàng",
                            Status = (int)HttpStatusCode.OK
                        };
                    }
                    var newOrderItemEntity = _mapper.Map<OrderItem>(newOrderItem);

                    _context.OrderItems.Add(newOrderItemEntity);
                    await _context.SaveChangesAsync();

                    return new ApiResponse<OrderItemProfile>
                    {
                        Success = true,
                        Message = "Đã thêm sản phẩm vào giỏ hàng.",
                        Data = _mapper.Map<OrderItemProfile>(newOrderItemEntity),
                        Status = (int)HttpStatusCode.OK
                    };
                };

                if (orderItem.Quantity <= 0)
                {
                    _context.OrderItems.Remove(orderItemEntity);
                }
                else
                {
                    int q = orderItem.Quantity < orderItemEntity.Quantity ? -1 : 1;

                    var isUpdatedOrder = await UpdateTotalAmount(null, orderItem, q);
                    if (!isUpdatedOrder)
                    {
                        return new ApiResponse<OrderItemProfile>
                        {
                            Success = false,
                            Message = "Lỗi cập nhật đơn hàng",
                            Status = (int)HttpStatusCode.OK
                        };
                    }
                    _mapper.Map(orderItem, orderItemEntity);
                    _context.OrderItems.Update(orderItemEntity);
                }

                await _context.SaveChangesAsync();

                return new ApiResponse<OrderItemProfile>
                {
                    Success = true,
                    Message = "Cập nhật đơn hàng thành công.",
                    Data = _mapper.Map<OrderItemProfile>(orderItemEntity),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderItemProfile>
                {
                    Success = false,
                    Message = "OrderService - UpdateOrderItem: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<OrderProfile>> GetOrderByID(int? orderID)
        {
            try
            {
                await Task.CompletedTask;
                var order = await _context.Orders.FindAsync(orderID);
                if (order == null)
                {
                    return new ApiResponse<OrderProfile>
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn hàng",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var orderItems = await _context.OrderItems.Where(x => x.OrderID == orderID).ToListAsync();
                var orderMapping = _mapper.Map<OrderProfile>(order);
                if(orderItems.Any())
                {
                    orderMapping.OrderItems = orderItems.Select(x => _mapper.Map<OrderItemProfile>(x)).ToList();
                }

                return new ApiResponse<OrderProfile>
                {
                    Success = true,
                    Message = "Tìm thấy đơn hàng",
                    Data = orderMapping,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OrderProfile>
                {
                    Success = false,
                    Message = "OrderService - GetOrderByID: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
