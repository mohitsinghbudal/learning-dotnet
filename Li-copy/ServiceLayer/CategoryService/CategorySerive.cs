using Li_copy.DataLayer.CategoryDLL;
using Li_copy.I_InterfaceLayer.CategoryInterface;
using Li_copy.Models.Category;

namespace Li_copy.ServiceLayer.CategoryService
{
    public class CategorySerive : ICategoryServices
    {
        private readonly ICategoryDLL _ICategoryDLL;
        public CategorySerive(ICategoryDLL icategoryDLL)
        {
            _ICategoryDLL = icategoryDLL;
        }
        public async Task<int> getCountAsync()
        {
            return await _ICategoryDLL.GetCountAsync();
        }
        public async Task<IEnumerable<Category>> GetCategoryAsync()
        {
            return await _ICategoryDLL.GetCategoryAsync();
        }
        public async Task<string?> AddCategoryAsync(string name)
        {
            return await _ICategoryDLL.AddCategoryAsync(name);
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            int rowsAffected = await _ICategoryDLL.DeleteCategoryAsync(id);

            return rowsAffected > 0;
        }
    }
}
