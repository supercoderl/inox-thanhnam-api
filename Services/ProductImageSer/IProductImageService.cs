using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.ProductImage;

namespace InoxThanhNamServer.Services.ProductImageSer
{
    public interface IProductImageService
    {
        Task<ApiResponse<List<ProductImageProfile>>> GetImagesByProduct(int ProductID);

        Task<ApiResponse<ProductImageProfile>> UploadImage(IFormFile file, CreateProductImageRequest request);
        Task<ApiResponse<ProductImageProfile>> DeleteImage(int ImageID);
    }
}
