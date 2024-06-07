using AutoMapper;
using InoxThanhNamServer.Datas.Category;
using InoxThanhNamServer.Datas.Contact;
using InoxThanhNamServer.Datas.Discount;
using InoxThanhNamServer.Datas.Notification;
using InoxThanhNamServer.Datas.Order;
using InoxThanhNamServer.Datas.Product;
using InoxThanhNamServer.Datas.ProductImage;
using InoxThanhNamServer.Datas.ProductReview;
using InoxThanhNamServer.Datas.Role;
using InoxThanhNamServer.Datas.User;
using InoxThanhNamServer.Datas.UserAddress;
using InoxThanhNamServer.Models;

namespace InoxThanhNamServer.Datas.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Models.User, UserCreateRequest>();
            CreateMap<UserCreateRequest, Models.User>();
            CreateMap<UserProfile, Models.User>();
            CreateMap<Models.User, UserProfile>();
            CreateMap<Models.User, UserUpdateRequest>();
            CreateMap<UserUpdateRequest, Models.User>();

            CreateMap<Models.Role, RoleProfile>();
            CreateMap<RoleProfile, Models.Role>();
            CreateMap<Models.Role, CreateRoleRequest>();
            CreateMap<CreateRoleRequest, Models.Role>();

            CreateMap<UserRole, UserRoleCreateRequest>();
            CreateMap<UserRoleCreateRequest, UserRole>();

            CreateMap<Models.Product, CreateProductRequest>();
            CreateMap<CreateProductRequest, Models.Product>();
            CreateMap<ProductProfile, Models.Product>();
            CreateMap<Models.Product, ProductProfile>();
            CreateMap<Models.Product, UpdateProductRequest>();
            CreateMap<UpdateProductRequest, Models.Product>();

            CreateMap<Models.ProductImage, ProductImageProfile>();
            CreateMap<ProductImageProfile, Models.ProductImage>();
            CreateMap<Models.ProductImage, CreateProductImageRequest>();
            CreateMap<CreateProductImageRequest, Models.ProductImage>();

            CreateMap<Models.Order, CreateOrderRequest>();
            CreateMap<CreateOrderRequest, Models.Order>();
            CreateMap<Models.OrderItem, CreateOrderItemRequest>();
            CreateMap<CreateOrderItemRequest, Models.OrderItem>();
            CreateMap<Models.Order, UpdateOrderRequest>();
            CreateMap<UpdateOrderRequest, Models.Order>();
            CreateMap<Models.OrderItem, UpdateOrderItemRequest>();
            CreateMap<UpdateOrderItemRequest, Models.OrderItem>();
            CreateMap<Models.Order, OrderProfile>();
            CreateMap<OrderProfile, Models.Order>();
            CreateMap<Models.OrderItem, OrderItemProfile>();
            CreateMap<OrderItemProfile, Models.OrderItem>();

            CreateMap<Models.Contact, CreateContactRequest>();
            CreateMap<CreateContactRequest, Models.Contact>();
            CreateMap<Models.Contact, ContactProfile>();
            CreateMap<ContactProfile, Models.Contact>();

            CreateMap<Models.UserAddress, AddressProfile>();
            CreateMap<AddressProfile, Models.UserAddress>();
            CreateMap<Models.UserAddress, AddressUpdateRequest>();
            CreateMap<AddressUpdateRequest, Models.UserAddress>();
            CreateMap<Models.UserAddress, CreateAddressRequest>();
            CreateMap<CreateAddressRequest, Models.UserAddress>();

            CreateMap<Models.Category, CategoryProfile>();
            CreateMap<CategoryProfile, Models.Category>();

            CreateMap<Models.Discount, DiscountProfile>();
            CreateMap<DiscountProfile, Models.Discount>();
            CreateMap<Models.Discount, CreateDiscountRequest>();
            CreateMap<CreateDiscountRequest, Models.Discount>();
            CreateMap<Models.Discount, UpdateDiscountRequest>();
            CreateMap<UpdateDiscountRequest, Models.Discount>();

            CreateMap<Models.Notification, NotificationProfile>();
            CreateMap<NotificationProfile, Models.Notification>();
            CreateMap<Models.Notification, UpdateNotificationRequest>();
            CreateMap<UpdateNotificationRequest, Models.Notification>();

            CreateMap<Models.ProductReview, ProductReviewProfile>();
            CreateMap<ProductReviewProfile, Models.ProductReview>();
            CreateMap<Models.ProductReview, CreateProductReviewRequest>();
            CreateMap<CreateProductReviewRequest, Models.ProductReview>(); 
            CreateMap<Models.ProductReview, UpdateProductReviewRequest>();
            CreateMap<UpdateProductReviewRequest, Models.ProductReview>();
        }
    }
}
