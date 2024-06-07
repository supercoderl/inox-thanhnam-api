using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Datas.ProductReview;
using InoxThanhNamServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Reflection;

namespace InoxThanhNamServer.Services.ProductReviewSer
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;

        public ProductReviewService(InoxEcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ApiResponse<ProductReviewProfile>> CreateReview(CreateProductReviewRequest request)
        {
            try
            {
                await Task.CompletedTask;
                var productReview = _mapper.Map<ProductReview>(request);
                await _context.ProductReviews.AddAsync(productReview);
                await _context.SaveChangesAsync();
                return new ApiResponse<ProductReviewProfile>
                {
                    Success = true,
                    Message = "Đánh giá thành công",
                    Data = _mapper.Map<ProductReviewProfile>(productReview),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductReviewProfile>
                {
                    Success = false,
                    Message = "Product Review Service - Create Review: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<ProductReviewProfile>> DeleteReview(int productReviewID)
        {
            try
            {
                await Task.CompletedTask;
                var productReview = await _context.ProductReviews.FindAsync(productReviewID);

                if (productReview == null)
                {
                    return new ApiResponse<ProductReviewProfile>
                    {
                        Success = false,
                        Message = "Bài đánh giá này không tồn tại",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                _context.ProductReviews.Remove(productReview);
                await _context.SaveChangesAsync();
                return new ApiResponse<ProductReviewProfile>
                {
                    Success = true,
                    Message = "Xóa bài đánh giá thành công",
                    Data = _mapper.Map<ProductReviewProfile>(productReview),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductReviewProfile>
                {
                    Success = false,
                    Message = "Product Review Service - Delete Review: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<ProductReviewProfile>>> GetReviews(int productID)
        {
            try
            {
                await Task.CompletedTask;
                var productReviews = await _context.ProductReviews.Where(p => p.ProductID == productID).OrderByDescending(p => p.ReviewDate).ToListAsync();
                if (!productReviews.Any())
                {
                    return new ApiResponse<List<ProductReviewProfile>>
                    {
                        Success = false,
                        Message = "Chưa có bài đánh giá nào",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                List<ProductReviewProfile?> productReviewsProfile = productReviews.Select(p => _mapper.Map<ProductReviewProfile>(p)).ToList();
      
                return new ApiResponse<List<ProductReviewProfile>>
                {
                    Success = true,
                    Message = "Lấy danh sách bài đánh giá",
                    Data = productReviewsProfile,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductReviewProfile>>
                {
                    Success = false,
                    Message = "Product Review Service - Get Reviews: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<ProductReviewProfile>> UpdateReview(int productReviewID, UpdateProductReviewRequest request)
        {
            try
            {
                await Task.CompletedTask;
                if(productReviewID != request.ReviewID)
                {
                    return new ApiResponse<ProductReviewProfile>
                    {
                        Success = false,
                        Message = "Bài đánh giá không hợp lệ",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var productReview = await _context.ProductReviews.FindAsync(productReviewID);
                if(productReview == null)
                {
                    return new ApiResponse<ProductReviewProfile>
                    {
                        Success = false,
                        Message = "Bài đánh giá không tồn tại",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                _mapper.Map(request, productReview);
                _context.ProductReviews.Update(productReview);
                await _context.SaveChangesAsync();

                return new ApiResponse<ProductReviewProfile>
                {
                    Success = true,
                    Message = "Cập nhật bài đánh giá thành công",
                    Data = _mapper.Map<ProductReviewProfile>(productReview),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<ProductReviewProfile?> FilterReview(List<ProductReviewProfile?> productReviews, FilterProductReview filter)
        {
            if (filter.ProductID != null)
            {
                productReviews = productReviews.Where(p => p.ProductID == filter.ProductID).ToList();
            }
            return productReviews;
        }
    }
}
