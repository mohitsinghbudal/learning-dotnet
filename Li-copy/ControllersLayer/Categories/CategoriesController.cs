using Li_copy.I_InterfaceLayer.CategoryInterface;
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
        [HttpGet]
        public async Task<IActionResult> GetCatAsync()
        {
           
            var categories = await _services.GetCategoryAsync();
            return Ok(categories);
        }
        [HttpGet("count")]
        public async Task<int> getCount()
        {
            return await _services.getCountAsync();
        }
    }
    
}

