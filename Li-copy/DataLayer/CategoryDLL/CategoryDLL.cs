using Dapper;
using Li_copy.I_InterfaceLayer.CategoryInterface;
using Li_copy.Models.Category;
using System.Data;

namespace Li_copy.DataLayer.CategoryDLL
{
    public class CategoryDLL : ICategoryDLL
    {
        private readonly IDbConnection _conn;

        public CategoryDLL(IDbConnection conn)
        {
            _conn = conn;
        }

        // Changed return type to IEnumerable<Category>
        public async Task<IEnumerable<Category>> GetCategoryAsync()
        {
            string sql = @"SELECT * FROM Categories";

            return await _conn.QueryAsync<Category>(sql);
        }

        public async Task<int> GetCountAsync()
        {
            string sql = @"select count(*) from categories";

            // QuerySingleAsync<int> returns a single integer result for the count
            return await _conn.QuerySingleAsync<int>(sql);
        }
        public async Task<string?> AddCategoryAsync(string name)
        {
            string sql = @"
        INSERT INTO Categories (Name)
        VALUES (@name)";

            int rowsAffected = await _conn.ExecuteAsync(sql, new { name });

            return rowsAffected > 0 ? "Category added successfully" : null;
        }

        public async Task<int> DeleteCategoryAsync(int id)
        {
            string sql = @"
        DELETE FROM Categories
        WHERE Id = @id";

            return await _conn.ExecuteAsync(sql, new { id });
        }
    }
}