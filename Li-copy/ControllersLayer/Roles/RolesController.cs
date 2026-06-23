using Li_copy.I_InterfaceLayer.RoleInterface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Li_copy.ControllersLayer.Roles
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _rolesService;

        public RolesController(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles() 
        {
            var roles = await _rolesService.GetRolesAsync();
            return Ok(roles);
        }
    }
}
