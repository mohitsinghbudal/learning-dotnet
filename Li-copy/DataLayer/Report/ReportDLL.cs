using Li_copy.I_InterfaceLayer.ReportInterface;
using System.Data;
using Li_copy.Models.Book;
using Dapper;

namespace Li_copy.DataLayer.Report
{
    public class ReportDLL : IReportDLL
    {
        private readonly IDbConnection _dbConn;
        public ReportDLL(IDbConnection dbconn)
        {
            _dbConn = dbconn;
        }
        public async Task<IEnumerable<Book>> GetMostBorrowedBooksAsync()
        {
            return await _dbConn.QueryAsync<Book>(
                "sp_GetMostBorrowedBooks",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
