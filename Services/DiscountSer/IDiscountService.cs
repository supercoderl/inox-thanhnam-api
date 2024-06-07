using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Discount;

namespace InoxThanhNamServer.Services.DiscountSer
{
    public interface IDiscountService
    {
        Task<ApiResponse<List<DiscountProfile>>> GetDiscounts(FilterDiscount? filter);
        Task<ApiResponse<DiscountProfile>> CreateDiscount(CreateDiscountRequest request);

        Task<ApiResponse<DiscountProfile>> UpdateDiscount(int discountID, UpdateDiscountRequest request);
    }
}
