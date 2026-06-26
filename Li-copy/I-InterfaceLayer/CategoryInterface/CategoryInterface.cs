using Li_copy.Models.Category;

namespace Li_copy.I_InterfaceLayer.CategoryInterface
{
    public interface ICategoryDLL
    {
         Task<IEnumerable<Category>> GetCategoryAsync();
        Task<int> GetCountAsync();
        Task<string?> AddCategoryAsync(string name);
        Task<int> DeleteCategoryAsync(int id);

    }
    public interface ICategoryServices
    {
        Task<int> getCountAsync();
        Task<IEnumerable<Category>> GetCategoryAsync();

        Task<string?> AddCategoryAsync(string name);
        Task<bool> DeleteCategoryAsync(int id);
    
}
}
