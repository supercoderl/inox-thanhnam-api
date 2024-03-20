using InoxThanhNamServer.Datas.User;
using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.UserAddress;

namespace InoxThanhNamServer.Services.UserAddress
{
    public interface IUserAddressService
    {
        Task<ApiResponse<AddressProfile>> GetAddressByUser(Guid UserID);
        Task<ApiResponse<AddressUpdateRequest>> UpdateAddress(int AddressID, AddressUpdateRequest address);
    }
}
