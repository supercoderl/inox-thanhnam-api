using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Category;
using InoxThanhNamServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InoxThanhNamServer.Services.CategorySer
{
    public class CategoryService : ICategoryService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;

        public CategoryService(InoxEcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ApiResponse<List<CategoryProfile>>> GetCategories()
        {
            try
            {
                await Task.CompletedTask;
                var categories = await _context.Categories.ToListAsync();
                if(!categories.Any())
                {
                    return new ApiResponse<List<CategoryProfile>>
                    {
                        Success = true,
                        Status = (int)HttpStatusCode.OK,
                        Message = "Không có loại sản phẩm."
                    };
                }
                return new ApiResponse<List<CategoryProfile>>
                {
                    Success = true,
                    Status = (int)HttpStatusCode.OK,
                    Data = categories.Select(x => _mapper.Map<CategoryProfile>(x)).ToList(),
                    Message = "Danh sách phân loại."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryProfile>>
                {
                    Success = false,
                    Status = (int)HttpStatusCode.InternalServerError,
                    Message = "CategoryService - GetCategories: " + ex.Message
                };
            }
        }
    }
}
