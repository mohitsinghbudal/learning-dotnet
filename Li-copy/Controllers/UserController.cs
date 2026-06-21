
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using Li_copy.Models;

namespace Li_copy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDbConnection _dbConn;

        public UserController(IDbConnection dbconn)
        {
            _dbConn = dbconn;
        }

        ///this is the first controller that just retuns the written content
        [HttpGet]
        public IActionResult getMethod()
        {
            return Ok("this is the get method");
        }

        //this is second controller with the database connected and swagger to check the api
        [HttpGet("userData")]
        public async Task<IActionResult> GetUser()
        {
            

            try
            {

                string sql = "SELECT * FROM users";

                var users = await _dbConn.QueryAsync<Users>(sql);

                if (users == null || !users.Any())
                {
                    return NotFound("No users found in the database.");
                }

                // Return the actual users data to Swagger/Client, not just a string
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exact error details to your console
                Console.WriteLine("Database error encountered: " + ex.Message);

                // Return an actual HTTP response so the API call doesn't hang or crash
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while connecting to the database." + ex.Message);
            }
        }
    }
}
