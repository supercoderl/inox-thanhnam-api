using InoxThanhNamServer.Datas;
using InoxThanhNamServer.Datas.Role;

namespace InoxThanhNamServer.Services.RoleSer
{
    public interface IRoleService
    {
        Task<ApiResponse<List<RoleProfile>>> GetRoles();
        Task<ApiResponse<RoleProfile>> CreateRole(CreateRoleRequest newRole);
        Task<ApiResponse<RoleProfile>> UpdateRole(int roleID, RoleProfile role);
        Task<ApiResponse<Object>> DeleteRole(int roleID);
        Task<ApiResponse<RoleProfile>> GetRoleByID(int roleID);
        Task<ApiResponse<List<object>>> GetRolesMapUser();
        Task<ApiResponse<object>> CreateRolesMapUser(UserRoleCreateRequest rolesMapUser);
        Task<ApiResponse<object>> DeleteRoleMapUser(int userRoleID);
    }
}
