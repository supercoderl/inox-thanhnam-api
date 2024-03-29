using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Discount;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InoxThanhNamServer.Services.DiscountSer
{
    public class DiscountService : IDiscountService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;

        public DiscountService(InoxEcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<DiscountProfile>> CreateDiscount(CreateDiscountRequest request)
        {
            try
            {
                await Task.CompletedTask;
                var discountEntity = _mapper.Map<Discount>(request);
                await _context.Discounts.AddAsync(discountEntity);
                await _context.SaveChangesAsync();
                return new ApiResponse<DiscountProfile>
                {
                    Success = true,
                    Message = "Tạo mã giảm giá thành công",
                    Data = _mapper.Map<DiscountProfile>(discountEntity),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<DiscountProfile>
                {
                    Success = false,
                    Message = "Discount Serivce - CreateDiscount: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<DiscountProfile>>> GetDiscounts()
        {
            try
            {
                await Task.CompletedTask;
                var discounts = await _context.Discounts.ToListAsync();
                if (!discounts.Any())
                    return new ApiResponse<List<DiscountProfile>>
                    {
                        Success = false,
                        Message = "Không có mã giảm giá nào",
                        Status = (int)HttpStatusCode.OK
                    };
                return new ApiResponse<List<DiscountProfile>>
                {
                    Success = true,
                    Message = "Tìm thấy mã giảm giá.",
                    Data = discounts.Select(x => _mapper.Map<DiscountProfile>(x)).ToList(),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<DiscountProfile>>
                {
                    Success = false,
                    Message = "Discount Serivce - GetDiscounts: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<DiscountProfile>> UpdateDiscount(int discountID, UpdateDiscountRequest request)
        {
            try
            {
                await Task.CompletedTask;

                if (discountID != request.DiscountID)
                {
                    return new ApiResponse<DiscountProfile>
                    {
                        Success = false,
                        Message = "Mã giảm giá không đúng.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                var discountEntity = await _context.Discounts.FindAsync(discountID);

                if (discountEntity == null)
                {
                    return new ApiResponse<DiscountProfile>
                    {
                        Success = false,
                        Message = "Không thể cập nhật vì mã giảm giá không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                _mapper.Map(request, discountEntity);
                _context.Discounts.Update(discountEntity);
                await _context.SaveChangesAsync();

                return new ApiResponse<DiscountProfile>
                {
                    Success = true,
                    Message = "Cập nhật mã giảm giá thành công.",
                    Data = _mapper.Map<DiscountProfile>(discountEntity),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<DiscountProfile>
                {
                    Success = false,
                    Message = "DiscountService - UpdateDiscount: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
