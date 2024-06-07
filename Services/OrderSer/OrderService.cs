using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Discount;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Models;
using InoxThanhNamServer.Services.ProductSer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NPOI.SS.Formula.Functions;
using System.Globalization;
using System.Net;
using System.Reflection;

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

        private async Task<bool> UpdateTotalAmount(CreateOrderItemRequest? request, UpdateOrderItemRequest? request1, int q, OrderEnum? type)
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

                switch (type)
                {
                    case OrderEnum.CREATE:
                        order.Data.ProductQuantity += 1;
                        break;
                    case OrderEnum.DELETE:
                        order.Data.ProductQuantity -= 1;
                        break;
                    default:
                        break;
                }

                var updateOrder = new UpdateOrderRequest
                {
                    OrderID = order.Data.OrderID,
                    UserID = order.Data.UserID,
                    OrderDate = DateTime.Now,
                    TotalAmount = order.Data.TotalAmount + q * product.Data.Price,
                    ProductQuantity = order.Data.ProductQuantity,
                    Status = order.Data.Status,
                };
                var result = await UpdateOrder(request != null ? request.OrderID : request1.OrderID, updateOrder, false);
                return (result.Success && result.Data != null);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<OrderProfile?> UpdateTotalOrder(OrderProfile? order)
        {
            try
            {
                await Task.CompletedTask;

                if (order == null) return order;

                var orderItems = await GetOrderItemByUser(order.OrderID);

                if (orderItems.Data == null || !orderItems.Data.Any())
                {
                    return order;
                }

                double total = 0;

                foreach (var item in orderItems.Data)
                {
                    if(item.Product != null)
                    {
                        total += item.Product.Price * item.Quantity;
                    }
                }

                order.TotalAmount = total;
                return order;
            }
            catch (Exception)
            {
                return order;
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
                    var q = 1;
                    var result = await UpdateTotalAmount(orderItem, null, q, OrderEnum.UPDATE);
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

                var isUpdatedOrder = await UpdateTotalAmount(orderItem, null, orderItem.Quantity, OrderEnum.CREATE);
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

                var isUpdatedOrder = await UpdateTotalAmount(null, _mapper.Map<UpdateOrderItemRequest>(orderItem), -(orderItem.Quantity), OrderEnum.DELETE);
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

        public async Task<ApiResponse<OrderProfile>> GetOrderByUser(OrderParameters param)
        {
            try
            {
                var query = _context.Orders.Where(x => x.Status == 0);

                if (param.UserID != null)
                {
                    query = query.Where(x => x.UserID == Guid.Parse(param.UserID));
                }
                else if (param.SessionID != null)
                {
                    query = query.Where(x => x.SessionID == Guid.Parse(param.SessionID));
                }

                var order = await query.FirstOrDefaultAsync();

                if (order == null)
                {
                    var newOrder = new CreateOrderRequest
                    {
                        UserID = param.UserID != null ? Guid.Parse(param.UserID) : null,
                        SessionID = param.SessionID != null ? Guid.Parse(param.SessionID) : null,
                        TotalAmount = 0,
                        Status = 0,
                    };
                    return await CreateOrder(newOrder);
                }

                var orderProfile = _mapper.Map<OrderProfile>(order);

                orderProfile = await UpdateTotalOrder(orderProfile);

                return new ApiResponse<OrderProfile>
                {
                    Success = true,
                    Message = "Tìm thấy giỏ hàng.",
                    Data = orderProfile,
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

                var orderItemsProfile = orderItems.Select(x => _mapper.Map<OrderItemProfile>(x)).ToList();

                foreach(var item in orderItemsProfile)
                {
                    if (item != null)
                    {
                        var products = await _productService.GetProductByID(item.ProductID);
                        if (products.Success && products.Data != null)
                        {
                            item.Product = products.Data;
                        }
                    }
                }

                return new ApiResponse<List<OrderItemProfile>>
                {
                    Success = true,
                    Message = "Tìm thấy các đơn hàng.",
                    Data = orderItemsProfile,
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

        public async Task<ApiResponse<List<OrderProfile>>> GetOrders(FilterOrder? filter)
        {
            try
            {
                await Task.CompletedTask;
                /*                var orders = await _context.Orders.FromSqlRaw("EXEC sp_get_orders @Status, @FromDate, @ToDate",
                                                new SqlParameter("@Status", status ?? (object)DBNull.Value),
                                                new SqlParameter("@FromDate", fromDate ?? (object)DBNull.Value),
                                                new SqlParameter("@ToDate", toDate ?? (object)DBNull.Value)).ToListAsync();*/

                var orders = await _context.Orders.OrderByDescending(o => o.OrderDate).ToListAsync();

                if(!orders.Any())
                {
                    return new ApiResponse<List<OrderProfile>>
                    {
                        Success = false,
                        Message = "Đơn hàng trống",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var ordersProfile = orders.Select(o => _mapper.Map<OrderProfile>(o)).ToList();

                if (filter != null) ordersProfile = FilterOrder(ordersProfile, filter);

                return new ApiResponse<List<OrderProfile>>
                {
                    Success = true,
                    Message = $"Tìm thấy ${ordersProfile.Count()} đơn hàng",
                    Data = ordersProfile,
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

        public async Task<ApiResponse<OrderProfile>> UpdateOrder(int? orderID, UpdateOrderRequest order, bool isCheckout)
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

                if(isCheckout)
                {
                    var error = await SubtractProductQuantity(orderEntity.OrderID);

                    if (!string.IsNullOrEmpty(error))
                    {
                        return new ApiResponse<OrderProfile>
                        {
                            Success = false,
                            Message = error,
                            Status = (int)HttpStatusCode.OK
                        };
                    }
                }

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

        private async Task<string> SubtractProductQuantity(int orderID)
        {
            try
            {
                await Task.CompletedTask;
                var orderItems = await GetOrderItemByUser(orderID);
                if (orderItems == null || orderItems.Data == null) return string.Empty;
                string error = await CheckQuantity(orderItems.Data);
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }
                foreach (var item in orderItems.Data)
                {
                    var result = await _productService.UpdateQuantityProduct(item.ProductID, item.Quantity);
                    if (!result.Success) return result.Message;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private async Task<string> CheckQuantity(List<OrderItemProfile> orderItems)
        {
            try
            {
                foreach (var item in orderItems)
                {
                    var result = await _productService.GetProductByID(item.ProductID);
                    if (result.Success && result.Data != null)
                    {
                        result.Data.Quantity = result.Data.Quantity - item.Quantity;
                        if (result.Data.Quantity < 0)
                        {
                            return result.Data.Quantity + item.Quantity == 0 ? $"Sản phẩm {result.Data.Name} hiện đã hết hàng." : $"Sản phẩm {result.Data.Name} chỉ còn lại {result.Data.Quantity + item.Quantity} cái.";
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
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
                    return new ApiResponse<OrderItemProfile>
                    {
                        Success = false,
                        Message = "Lỗi đơn hàng.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                if (orderItem.Quantity <= 0)
                {
                    await DeleteOrderItem(orderItemID);
                }
                else
                {
                    int q = orderItem.Quantity < orderItemEntity.Quantity ? -1 : 1;

                    var isUpdatedOrder = await UpdateTotalAmount(null, orderItem, q, OrderEnum.UPDATE);
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

        private List<OrderProfile> FilterOrder(List<OrderProfile> orders, FilterOrder filter)
        {
            if (filter.OrderDateFrom is not null && filter.OrderDateTo is not null)
            {
                DateTime orderDateFrom = DateTime.ParseExact(filter.OrderDateFrom, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime orderDateTo = DateTime.ParseExact(filter.OrderDateTo, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                orders = orders.Where(x =>
                    x.OrderDate >= orderDateFrom
                    &&
                    x.OrderDate <= orderDateTo
               ).ToList();
            }
            if(filter.Status != null && filter.Status != -1)
            {
                orders = orders.Where(x => x.Status == filter.Status).ToList();
            }
            if(filter.TotalMin != null && filter.TotalMax != null)
            {
                orders = orders.Where(x => x.TotalAmount >= filter.TotalMin && x.TotalAmount <= filter.TotalMax).ToList();
            }
            if (filter.SortType is not null)
            {
                PropertyInfo propertyInfo = typeof(OrderProfile).GetProperty(char.ToUpper(filter.SortType[0]) + filter.SortType.Substring(1))!;
                if(propertyInfo != null)
                {
                    switch (filter.SortFrom)
                    {
                        case "ascending":
                            orders = orders.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                            break;
                        default:
                            orders = orders.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                            break;
                    }
                }
            }
            if (filter.SearchText is not null)
                orders = orders.Where(x =>
                    x.Fullname != null && x.Fullname.ToLower().Contains(filter.SearchText.ToLower())
                ).ToList();
            if (!filter.IsZeroStatus)
            {
                orders = orders.Where(x => x.Status != 0).ToList();
            }
            return orders;
        }
    }
}
