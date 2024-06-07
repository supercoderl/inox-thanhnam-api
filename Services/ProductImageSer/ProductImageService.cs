using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Datas.ProductImage;
using InoxThanhNamServer.Models;
using InoxThanhNamServer.Services.FileSer;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InoxThanhNamServer.Services.ProductImageSer
{
    public class ProductImageService : IProductImageService
    {
        private readonly InoxEcommerceContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _web;
        private readonly IFileService _fileService;

        public ProductImageService(InoxEcommerceContext context, IMapper mapper, IWebHostEnvironment web, IFileService fileService)
        {
            _context = context;
            _mapper = mapper;
            _web = web;
            _fileService = fileService;
        }

        public async Task<ApiResponse<ProductImageProfile>> DeleteImage(int ImageID)
        {
            try
            {
                await Task.CompletedTask;
                var productImage = await _context.ProductImages.FindAsync(ImageID);
                if (productImage == null)
                {
                    return new ApiResponse<ProductImageProfile>
                    {
                        Success = true,
                        Message = "Không tồn tại hình ảnh này.",
                        Status = (int)HttpStatusCode.OK
                    };
                }
                if(!(productImage.ImageURL.Contains("https://") || productImage.ImageURL.Contains("http://"))) System.IO.File.Delete(Path.Combine(_web.WebRootPath, productImage.ImageURL));
                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();
                return new ApiResponse<ProductImageProfile>
                {
                    Success = true,
                    Message = "Đã xóa hình ảnh",
                    Data = _mapper.Map<ProductImageProfile>(productImage),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductImageProfile>
                {
                    Success = false,
                    Message = "ProductImageService - DeleteImage: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<string>> GetImageByProductId(int? productID)
        {
            try
            {
                await Task.CompletedTask;

                if (productID == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "ID không đúng",
                        Data = "https://upload.wikimedia.org/wikipedia/commons/1/14/No_Image_Available.jpg",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                var image = await _context.ProductImages.FirstOrDefaultAsync(i => i.ProductID == productID);
                if (image == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Không có hình",
                        Data = "https://upload.wikimedia.org/wikipedia/commons/1/14/No_Image_Available.jpg",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Lấy hình ảnh thành công",
                    Data = image.ImageURL,
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = true,
                    Message = "Product Image Service - Get Image By Product: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<List<ProductImageProfile>>> GetImagesByProduct(int ProductID)
        {
            try
            {
                await Task.CompletedTask;
                var images = await _context.ProductImages.Where(x => x.ProductID == ProductID).ToListAsync();
                if (!images.Any())
                {
                    return new ApiResponse<List<ProductImageProfile>>
                    {
                        Success = false,
                        Message = "Không có hình của sản phẩm này.",
                        Status = (int)HttpStatusCode.OK
                    };
                }

                return new ApiResponse<List<ProductImageProfile>>
                {
                    Success = true,
                    Message = "Lấy danh sách hình sản phẩm thành công.",
                    Data = images.Select(x => _mapper.Map<ProductImageProfile>(x)).ToList(),
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductImageProfile>>
                {
                    Success = false,
                    Message = "ProductImageService - GetImagesByProduct: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ApiResponse<ProductImageProfile>> UploadImage(IFormFile file, CreateProductImageRequest request)
        {
            try
            {
                await Task.CompletedTask;
                request.ImageURL = await _fileService.UploadFile(file, request.ImageName);
                var productImageEntity = _mapper.Map<ProductImage>(request);
                await _context.ProductImages.AddAsync(productImageEntity);
                await _context.SaveChangesAsync();
                return new ApiResponse<ProductImageProfile>
                {
                    Success = true,
                    Data = _mapper.Map<ProductImageProfile>(productImageEntity),
                    Message = "Tải ảnh lên thành công.",
                    Status = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductImageProfile>
                {
                    Success = false,
                    Message = "ProductImageService - UploadImage: " + ex.Message,
                    Status = (int)HttpStatusCode.InternalServerError
                };
            }
        }


    }
}
