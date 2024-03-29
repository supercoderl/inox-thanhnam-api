using InoxThanhNamServer.Datas.UserAddress;
using InoxThanhNamServer.Services.UserAddressSer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> GetAddressByUser() 
        {
            string? userID = User.FindFirstValue("UserID");
            if (userID is null)
            {
                return Unauthorized();
            }
            var result = await _userAddressService.GetAddressByUser(Guid.Parse(userID));
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-address")]
        public async Task<IActionResult> CreateUserAddress(CreateAddressRequest request)
        {
            string? userID = User.FindFirstValue("UserID");
            if (userID is null)
            {
                return Unauthorized();
            }
            request.UserID = Guid.Parse(userID);
            var result = await _userAddressService.CreateUserAddress(request); 
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
