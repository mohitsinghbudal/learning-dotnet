using Li_copy.I_InterfaceLayer.Login_Sign;
using Li_copy.I_InterfaceLayer.UserInterface;
using Li_copy.Models.Users;
using Li_copy.Models.DTO;
using Li_copy.I_InterfaceLayer.Jwt;

namespace Li_copy.ServiceLayer.LoginService
{
    public class UserServices : ILogSignReq

    {
            private readonly IJwtServices _jwtServices;
            private readonly IUserDLL _userDLL;

            public UserServices(IUserDLL userDLL, IJwtServices jwtServices)
            {
                _jwtServices = jwtServices;
                _userDLL = userDLL;
            }

        public async Task<LoginResDTO?> LoginAsync(LoginReqDTO req)
        {
            Console.WriteLine("reached service layer");

            var user = await _userDLL.GetUserByEmailAsync(req.Email);

            if (user == null)
                return null;

            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(
                    req.PasswordHash,
                    user.PasswordHash);

            if (!isPasswordValid)
                return null;

            var token = _jwtServices.GenerateTokenAsync(user);

            return new LoginResDTO
            {
                Token = token,
                Id = user.Id,
                FullName = user.FullName,
                Message = "Login successful",
                RoleId = user.RoleId
                
            };
        }

        public async Task<bool> SignupAsync(SignUpReqDTO request)
        {
            var existingUser = await _userDLL.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return false; // User already exists
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash,
                Phone = request.Phone,
                RoleId = request.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            // Saves user and updates user object with new Id
            await _userDLL.CreateUserAsync(user);

            return true; // Successfully created
        }

    }
}
