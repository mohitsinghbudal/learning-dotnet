using Dapper;
using Li_copy.Models.Users;
using Li_copy.I_InterfaceLayer.UserInterface;
using System.Data;
using System.Data.Common;

namespace Li_copy.DataLayer.UserDLL
{
    public class UserDLL : IUserDLL
    {
        private readonly IDbConnection _conn;
        public UserDLL(IDbConnection conn)
        {
            _conn = conn;
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            string sql = @"
                SELECT
                    Id,
                    FullName,
                    Email,
                    PasswordHash,
                    Phone,
                    RoleId,
                    CreatedAt
                FROM Users
                WHERE Email = @Email";

            return await _conn.QueryFirstOrDefaultAsync<User>(
                sql,
                new { Email = email });
        }
        public async Task<int> CreateUserAsync(User user)
        {
            string sql = @"
                INSERT INTO Users
                (
                    FullName,
                    Email,
                    PasswordHash,
                    Phone,
                    RoleId,
                    CreatedAt
                )
                VALUES
                (
                    @FullName,
                    @Email,
                    @PasswordHash,
                    @Phone,
                    @RoleId,
                    @CreatedAt
                );

                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _conn.ExecuteScalarAsync<int>(
                sql,
                user);
        } 
    }
}
