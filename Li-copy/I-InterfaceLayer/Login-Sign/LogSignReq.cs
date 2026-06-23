using Li_copy.Models.DTO;

namespace Li_copy.I_InterfaceLayer.Login_Sign
{
    public interface ILogSignReq
    {
        // Returns a JWT token string when login succeeds, null otherwise
        Task<string?> LoginAsync(LoginReqDTO request);

        // Creates a new user; returns true on success
        Task<bool> SignupAsync(SignUpReqDTO request);
    }
}
