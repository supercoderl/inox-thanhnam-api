using InoxThanhNamServer.Datas.User;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.UserAddress;

namespace InoxThanhNamServer.Services.UserAddressSer
{
    public interface IUserAddressService
    {
        Task<ApiResponse<AddressProfile>> GetAddressByUser(Guid UserID);

        Task<ApiResponse<AddressProfile>> CreateUserAddress(CreateAddressRequest request);
        Task<ApiResponse<AddressUpdateRequest>> UpdateAddress(int AddressID, AddressUpdateRequest address);
    }
}
