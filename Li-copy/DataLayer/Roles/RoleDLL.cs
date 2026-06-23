using Dapper;
using Li_copy.I_InterfaceLayer.RoleInterface;
using Li_copy.Models;
using Li_copy.Models.Roles;
using System.Data;
namespace Li_copy.DataLayer.Roles
{
    public class RoleDLL : IRolesDLL
    {
        private readonly IDbConnection _dbConn;

        public RoleDLL(IDbConnection dbconn)
        {
            _dbConn = dbconn;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {

            string sql = "SELECT * FROM Roles";

            return await _dbConn.QueryAsync<Role>(sql);
        
        }
    }
}
