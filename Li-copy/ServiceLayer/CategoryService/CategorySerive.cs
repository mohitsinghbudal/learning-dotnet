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
}
}
