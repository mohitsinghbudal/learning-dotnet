using Li_copy.Models.Roles;

namespace Li_copy.I_InterfaceLayer.RoleInterface
{
    public interface IRolesDLL
    {
        Task<IEnumerable<Role>> GetRolesAsync();
    }
    public interface IRolesService
    {
        Task<IEnumerable<Role>> GetRolesAsync();
    }
}
