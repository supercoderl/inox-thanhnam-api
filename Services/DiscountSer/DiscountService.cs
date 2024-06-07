using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Discount;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Datas.User;
using InoxThanhNamServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;

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

        public async Task<ApiResponse<List<DiscountProfile>>> GetDiscounts(FilterDiscount? filter)
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

                var discountsProfile = discounts.Select(d => _mapper.Map<DiscountProfile>(d)).ToList();

                if (filter != null) discountsProfile = FilterDiscount(discountsProfile, filter);

                return new ApiResponse<List<DiscountProfile>>
                {
                    Success = true,
                    Message = $"Tìm thấy {discountsProfile.Count()} mã giảm giá.",
                    Data = discountsProfile,
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

        private List<DiscountProfile> FilterDiscount(List<DiscountProfile> discounts, FilterDiscount filter)
        {
            if (filter.PercentageMin != null && filter.PercentageMax != null)
            {
                discounts = discounts.Where(d => d.Percentage >= filter.PercentageMin && d.Percentage <= filter.PercentageMax).ToList();
            }
            if (filter.DateExpire != null)
            {
                discounts = discounts.Where(d => d.DateExpire == filter.DateExpire).ToList();
            }
            if (filter.LimitedQuantity != null)
            {
                discounts = discounts.Where(d => d.LimitedQuantity <= filter.LimitedQuantity).ToList();
            }
            if (filter.Status != null && filter.Status != -1)
            {
                discounts = discounts.Where(d => (d.Active ? 1 : 0) == filter.Status).ToList();
            }
            if (filter.SortType is not null)
            {
                PropertyInfo? propertyInfo = typeof(DiscountProfile).GetProperty(char.ToUpper(filter.SortType[0]) + filter.SortType.Substring(1))!;
                if(propertyInfo != null)
                {
                    switch (filter.SortFrom)
                    {
                        case "ascending":
                            discounts = discounts.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                            break;
                        default:
                            discounts = discounts.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                            break;
                    }
                }
            }
            if (filter.SearchText is not null)
                discounts = discounts.Where(x =>
                    (x.Code != null && x.Code.ToLower().Contains(filter.SearchText.ToLower())) ||
                    (x.Name != null && x.Name.ToLower().Contains(filter.SearchText.ToLower()))
                ).ToList();
            return discounts;
        }
    }
}
