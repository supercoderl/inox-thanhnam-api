using AutoMapper;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Discount;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Models;
using InoxThanhNamServer.Services.ProductImageSer;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Reflection;

namespace InoxThanhNamServer.Services.ProductSer
{
    public class ProductService : IProductService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;
        private readonly IProductImageService _productImageService;

        public ProductService(InoxEcommerceContext context, IMapper mapper, IProductImageService productImageService)
        {
            _context = context;
            _mapper = mapper;
            _productImageService = productImageService;
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

                var productProfile = _mapper.Map<ProductProfile>(product);
                var imageResult = await _productImageService.GetImageByProductId(productProfile?.ProductID);

                if(imageResult != null && imageResult.Data != null)
                {
                    productProfile.ImageURL = imageResult.Data;
                }

                return new ApiResponse<ProductProfile>
                {
                    Success = true,
                    Message = "Tìm thấy sản phẩm.",
                    Data = productProfile,
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

        public async Task<ApiResponse<List<ProductProfile>>> GetProducts(FilterProduct? filter)
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

                var productsProfile = products.Select(p => _mapper.Map<ProductProfile>(p)).ToList();

                if (filter != null) productsProfile = FilterProduct(productsProfile, filter);

                foreach(var product in productsProfile)
                {
                    var imageResult = await _productImageService.GetImageByProductId(product.ProductID);
                    product.ImageURL = imageResult.Data;
                }

                return new ApiResponse<List<ProductProfile>>
                {
                    Success = true,
                    Message = $"Lấy danh sách {productsProfile.Count()} sản phẩm.",
                    Data = productsProfile,
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

        public async Task<ApiResponse<ProductProfile>> UpdateQuantityProduct(int productID, int quantity)
        {
            try
            {
                await Task.CompletedTask;
                var product = await GetProductByID(productID);
                if (product == null || product.Data == null)
                {
                    return new ApiResponse<ProductProfile>
                    {
                        Success = false,
                        Message = "Product does not exists!",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                Product? productEntity = _mapper.Map<Product>(product.Data);

                if(productEntity == null)
                {
                    return new ApiResponse<ProductProfile>
                    {
                        Success = false,
                        Message = "Lỗi mapping",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                productEntity.Quantity = productEntity.Quantity - quantity;

                return await UpdateProduct(productID, _mapper.Map<UpdateProductRequest>(productEntity));
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductProfile>
                {
                    Success = false,
                    Message = "Product Service - Update Quantity: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        private List<ProductProfile> FilterProduct(List<ProductProfile> products, FilterProduct filter)
        {
            if (filter.UpdatedDateFrom is not null && filter.UpdatedDateTo is not null)
            {
                DateTime updatedDateFrom = DateTime.ParseExact(filter.UpdatedDateFrom, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime updatedDateTo = DateTime.ParseExact(filter.UpdatedDateTo, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                products = products.Where(x => x.UpdatedAt != null &&
                    x.UpdatedAt >= updatedDateFrom
                    &&
                    x.UpdatedAt <= updatedDateTo
               ).ToList();
            }
            if (filter.PriceMin != null && filter.PriceMax != null)
            {
                products = products.Where(p => p.Price >= filter.PriceMin && p.Price <= filter.PriceMax).ToList();
            }
            if(filter.CategoryID != null && filter.CategoryID != -1)
            {
                products = products.Where(p => p.CategoryID ==  filter.CategoryID).ToList();
            }
            if (filter.ProductID != null)
            {
                products = products.Where(p => p.ProductID == filter.ProductID).ToList();
            }
            if (filter.SortType is not null)
            {
                PropertyInfo? propertyInfo = typeof(ProductProfile).GetProperty(char.ToUpper(filter.SortType[0]) + filter.SortType.Substring(1))!;
                if(propertyInfo != null)
                {
                    switch (filter.SortFrom)
                    {
                        case "ascending":
                            products = products.OrderBy(x => propertyInfo.GetValue(x, null)).ToList();
                            break;
                        default:
                            products = products.OrderByDescending(x => propertyInfo.GetValue(x, null)).ToList();
                            break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(filter.SearchText))
                products = products.Where(x =>
                    x.Name != null && x.Name.ToLower().Contains(filter.SearchText.ToLower())
                ).ToList();
            return products;
        }
    }
}
