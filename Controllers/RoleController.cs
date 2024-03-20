using InoxThanhNamServer.Datas.Role;
using InoxThanhNamServer.Services.RoleSer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InoxThanhNamServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("get-roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _roleService.GetRoles();
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-roles-map-user")]
        public async Task<IActionResult> GetRolesMapUser()
        {
            var result = await _roleService.GetRolesMapUser();
            return StatusCode(result.Status, result);
        }

        [HttpGet("get-role-by-id")]
        public async Task<IActionResult> GetRoleByID(int roleID)
        {
            var result = await _roleService.GetRoleByID(roleID);
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole(UserRoleCreateRequest newUserRole)
        {
            var result = await _roleService.CreateRolesMapUser(newUserRole);
            return StatusCode(result.Status, result);
        }

        [HttpPost("create-mapping")]
        public async Task<IActionResult> CreateRoleMapUser(UserRoleCreateRequest newUserRole)
        {
            var result = await _roleService.CreateRolesMapUser(newUserRole);
            return StatusCode(result.Status, result);
        }

        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateRole(int roleID, RoleProfile role)
        {
            var result = await _roleService.UpdateRole(roleID, role);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("delete-role/{roleID}")]
        public async Task<IActionResult> DeleteRole(int roleID)
        {
            var result = await _roleService.DeleteRole(roleID);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("delete-mapping/{userRoleID}")]
        public async Task<IActionResult> DeleteRoleMapUser(int userRoleID)
        {
            var result = await _roleService.DeleteRoleMapUser(userRoleID);
            return StatusCode(result.Status, result);
        }
    }
}
