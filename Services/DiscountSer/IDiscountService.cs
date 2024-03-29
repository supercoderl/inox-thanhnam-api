using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Discount;

namespace InoxThanhNamServer.Services.DiscountSer
{
    public interface IDiscountService
    {
        Task<ApiResponse<List<DiscountProfile>>> GetDiscounts();
        Task<ApiResponse<DiscountProfile>> CreateDiscount(CreateDiscountRequest request);

        Task<ApiResponse<DiscountProfile>> UpdateDiscount(int discountID, UpdateDiscountRequest request);
    }
}
