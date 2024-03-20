using InoxThanhNamServer.Datas.UserAddress;
using InoxThanhNamServer.Services.UserAddress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAddressController : ControllerBase
    {
        private readonly IUserAddressService _userAddressService;

        public UserAddressController(IUserAddressService userAddressService)
        {
            _userAddressService = userAddressService;
        }

        [HttpGet("get-address-by-user")]
        public async Task<IActionResult> GetAddressByUser(Guid UserID) 
        { 
            var result = await _userAddressService.GetAddressByUser(UserID);
            return StatusCode(result.Status, result);
        }

        [HttpPut("update-address/{AddressID}")]
        public async Task<IActionResult> UpdateAddress(int AddressID, AddressUpdateRequest address)
        {
            var result = await _userAddressService.UpdateAddress(AddressID, address);
            return StatusCode(result.Status, result);
        }
    }
}
