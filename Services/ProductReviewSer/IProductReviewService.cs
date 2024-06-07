using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.ProductReview;

namespace InoxThanhNamServer.Services.ProductReviewSer
{
    public interface IProductReviewService
    {
        Task<ApiResponse<List<ProductReviewProfile>>> GetReviews(int productID);
        Task<ApiResponse<ProductReviewProfile>> CreateReview(CreateProductReviewRequest request);
        Task<ApiResponse<ProductReviewProfile>> UpdateReview(int productReviewID, UpdateProductReviewRequest request);
        Task<ApiResponse<ProductReviewProfile>> DeleteReview(int productReviewID);
    }
}
