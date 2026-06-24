using Li_copy.Models.Category;

namespace Li_copy.I_InterfaceLayer.CategoryInterface
{
    public interface ICategoryDLL
    {
         Task<IEnumerable<Category>> GetCategoryAsync();
        Task<int> GetCountAsync();
    }
    public interface ICategoryServices
    {
        Task<int> getCountAsync();
        Task<IEnumerable<Category>> GetCategoryAsync();
    }
}
