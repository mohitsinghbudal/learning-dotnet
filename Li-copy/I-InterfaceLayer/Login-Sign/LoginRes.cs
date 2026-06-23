using Li_copy.Models.Users;

namespace Li_copy.I_InterfaceLayer.UserInterface
{
    public interface IUserDLL
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<int> CreateUserAsync(User user);
    }
}
