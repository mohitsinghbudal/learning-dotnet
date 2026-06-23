using Li_copy.Models.Users;

namespace Li_copy.I_InterfaceLayer.Jwt
{
    public interface IJwtServices
    {
        string GenerateTokenAsync(User user);
    }
}
