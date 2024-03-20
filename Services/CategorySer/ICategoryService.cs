using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Category;

namespace InoxThanhNamServer.Services.CategorySer
{
    public interface ICategoryService 
    {
        Task<ApiResponse<List<CategoryProfile>>> GetCategories();
    }
}
