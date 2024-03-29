using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InoxThanhNamServer.Services.ProductSer
{
    public class ProductService : IProductService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;

        public ProductService(InoxEcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ApiResponse<ProductProfile>> CreateProduct(CreateProductRequest product)
        {
            try
            {
                await Task.CompletedTask;
                var productEntity = _mapper.Map<Product>(product);
                await _context.Products.AddAsync(productEntity);
                await _context.SaveChangesAsync();


                return new ApiResponse<ProductProfile>
                {
                    Success = true,
                    Message = "Tạo sản phẩm thành công.",
                    Data = _mapper.Map<ProductProfile>(productEntity),
                    Status = (int)HttpStatusCode.Created
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductProfile>
                {
                    Success = false,
                    Message = "ProductService - CreateProduct: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<object>> DeleteProduct(int productID)
        {
            try
            {
                await Task.CompletedTask;
                await _context.Database.ExecuteSqlInterpolatedAsync($"sp_delete_product {productID}");
                await _context.SaveChangesAsync();
                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "Xóa sản phẩm thành công.",
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Object>
                {
                    Success = false,
                    Message = "ProductService - DeleteProduct: " + ex,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<ProductProfile>> GetProductByID(int? productID)
        {
            try
            {
                await Task.CompletedTask;
                var product = await _context.Products.FindAsync(productID);
                if (product == null)
                {
                    return new ApiResponse<ProductProfile>
                    {
                        Success = false,
                        Message = "Không có sản phẩm.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                return new ApiResponse<ProductProfile>
                {
                    Success = true,
                    Message = "Tìm thấy sản phẩm.",
                    Data = _mapper.Map<ProductProfile>(product),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductProfile>
                {
                    Success = false,
                    Message = "ProductService - GetProductByID: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<ProductProfile>>> GetProducts()
        {
            try
            {
                await Task.CompletedTask;
                var products = await _context.Products.ToListAsync();
                if (!products.Any())
                {
                    return new ApiResponse<List<ProductProfile>>
                    {
                        Success = false,
                        Message = "Không có sản phẩm nào.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                return new ApiResponse<List<ProductProfile>>
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm thành công.",
                    Data = products.Select(x => _mapper.Map<ProductProfile>(x)).ToList(),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductProfile>>
                {
                    Success = false,
                    Message = "ProductService - GetProducts: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<ProductProfile>> UpdateProduct(int productID, UpdateProductRequest product)
        {
            try
            {
                await Task.CompletedTask;

                if (productID != product.ProductID)
                {
                    return new ApiResponse<ProductProfile>
                    {
                        Success = false,
                        Message = "Sản phẩm không đúng.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                var productEntity = await _context.Products.FindAsync(productID);

                if (productEntity == null)
                {
                    return new ApiResponse<ProductProfile>
                    {
                        Success = false,
                        Message = "Không thể cập nhật vì sản phâm không tồn tại.",
                        Status = (int)HttpStatusCode.OK
                    };
                };

                _mapper.Map(product, productEntity);
                _context.Products.Update(productEntity);
                await _context.SaveChangesAsync();

                return new ApiResponse<ProductProfile>
                {
                    Success = true,
                    Message = "Cập nhật sản phẩm thành công.",
                    Data = _mapper.Map<ProductProfile>(productEntity),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductProfile>
                {
                    Success = false,
                    Message = "ProductService - UpdateProduct: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}
