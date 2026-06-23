
//using Microsoft.AspNetCore.Mvc;
//using System.Data;
//using Dapper;
//using Li_copy.Models;

//namespace Li_copy.Controllers.Users
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly IDbConnection _dbConn;

//        public UserController(IDbConnection dbconn)
//        {
//            _dbConn = dbconn;
//        }

//        [HttpGet]
//        public IActionResult getMethod()
//        {
//            return Ok("this is the get method");
//        }

//        [HttpGet("userData")]
//        public async Task<IActionResult> GetUser()
//        {
            

//            try
//            {

//                string sql = "SELECT * FROM users";

//                var users = await _dbConn.QueryAsync<Users>(sql);

//                if (users == null || !users.Any())
//                {
//                    return NotFound("No users found in the database.");
//                }

//                return Ok(users);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Database error encountered: " + ex.Message);

//                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while connecting to the database." + ex.Message);
//            }
//        }
//    }
//}
