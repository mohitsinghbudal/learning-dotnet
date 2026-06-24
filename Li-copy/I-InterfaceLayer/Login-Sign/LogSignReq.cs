using Li_copy.Models.DTO;
using Li_copy.Models.Users;

namespace Li_copy.I_InterfaceLayer.Login_Sign
{
    public interface ILogSignReq
    {
        // Returns a JWT token string when login succeeds, null otherwise
        Task<LoginResDTO?> LoginAsync(LoginReqDTO request);

        // Creates a new user; returns true on success
        Task<bool> SignupAsync(SignUpReqDTO request);
    }
}

namespace Li_copy.I_InterfaceLayer.UserInterface
{
    public interface IUserDLL
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<User> CreateUserAsync(User user);
    }
}

