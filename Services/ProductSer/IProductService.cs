using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Product;

namespace InoxThanhNamServer.Services.ProductSer
{
    public interface IProductService
    {
        Task<ApiResponse<List<ProductProfile>>> GetProducts();
        Task<ApiResponse<ProductProfile>> CreateProduct(CreateProductRequest product);
        Task<ApiResponse<ProductProfile>> UpdateProduct(int productID, UpdateProductRequest product);
        Task<ApiResponse<Object>> DeleteProduct(int productID);
        Task<ApiResponse<ProductProfile>> GetProductByID(int productID);
    }
}
