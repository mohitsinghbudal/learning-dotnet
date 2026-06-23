using System.Security.Claims;

namespace Li_copy.Helper
{
    public static class ClaimsHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        public static int GetRoleId(ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst("RoleId")!.Value);
        }
    }
}
