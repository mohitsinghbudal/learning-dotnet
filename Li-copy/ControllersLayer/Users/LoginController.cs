using Li_copy.I_InterfaceLayer;
using Li_copy.I_InterfaceLayer.Login_Sign;
using Li_copy.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;

namespace Li_copy.Controllers.Users
{
        [Route("api/[controller]")]
        [ApiController]
        public class LoginController : ControllerBase
    {
        private readonly ILogSignReq _login;

        public LoginController (ILogSignReq login)
        {
            _login = login;
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginReqDTO request)
        {
            Console.WriteLine("login attempted");
            

            var result = await _login.LoginAsync(request);
            
            if(result == null)
            {
                return Unauthorized("Inavalid Email or Password");
            }

            return Ok(result);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpReqDTO request)
        {
            var success = await _login.SignupAsync(request);
            
            if(!success) 
            {
                return BadRequest(new { message = "User Already exists or invalid data" });
            }

            return Ok(new { message = "Registration successful!" });
        }


    }
}
