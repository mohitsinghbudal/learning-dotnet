using Li_copy.Helper;
using Li_copy.I_InterfaceLayer.CategoryInterface;
using Li_copy.I_InterfaceLayer.Jwt;
using Li_copy.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Li_copy.ControllersLayer.Categories
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {

        private readonly ICategoryServices _services;
        public CategoriesController(ICategoryServices services) 
        {
            _services = services;
            
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCatAsync()
        {


            var categories = await _services.GetCategoryAsync();
            return Ok(categories);
        }
        [AllowAnonymous]
        [HttpGet("count")]
        public async Task<int> getCount()
        {
            return await _services.getCountAsync();
        }


        [HttpPost("Add")]
        public async Task<IActionResult> AddCategoryAsync([FromBody] string category)
        {
            var result = await _services.AddCategoryAsync(category);
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var deleted = await _services.DeleteCategoryAsync(id);

            if (!deleted)
                return NotFound("Category not found.");

            return Ok("Category deleted successfully.");
        }

    }
    
}

