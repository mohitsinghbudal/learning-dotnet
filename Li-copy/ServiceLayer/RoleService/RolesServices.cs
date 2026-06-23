using Li_copy.DataLayer;
using Li_copy.I_InterfaceLayer;
using Li_copy.I_InterfaceLayer.RoleInterface;
using Li_copy.Models.Roles;
using Microsoft.AspNetCore.Identity;

namespace Li_copy.Services.RolesServices
{
        public class RoleService : IRolesService
    {
        private readonly IRolesDLL _roleDLL;
        public RoleService(IRolesDLL roleDLL)
        {
            _roleDLL = roleDLL;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _roleDLL.GetRolesAsync();
        }
    }
}
