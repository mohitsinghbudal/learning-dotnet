using Dapper;
using Li_copy.DataLayer.FineDLL;
using Li_copy.DataLayer.UserDLL;
using Li_copy.I_InterfaceLayer.ReportInterface;
using Li_copy.Models.Book;
using Li_copy.Models.DTO;
using System.Data;

namespace Li_copy.DataLayer.Report
{
    public class ReportDLL : IReportDLL
    {
        private readonly IDbConnection _dbConn;
        public ReportDLL(IDbConnection dbconn)
        {
            _dbConn = dbconn;
        }
        public async Task<IEnumerable<MostBorrowedBookDto>> GetMostBorrowedBooksAsync()
        {
            return await _dbConn.QueryAsync<MostBorrowedBookDto>(
                "sp_GetMostBorrowedBooks",
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<decimal> TotalFineCollectedAsync() // Changed to return single decimal/int
        {
            return await _dbConn.ExecuteScalarAsync<decimal>(
                "sp_TotalFineCollected", // Corrected SP Name
                commandType: CommandType.StoredProcedure
            );
        }
        //public async Task<IEnumerable<FineDto>> TotalFineMonthly(DateTime start, DateTime end)
        //{
        //    return await _dbConn.QueryAsync<FineDto>(
        //        "sp_GetTotalFineMonthly", // Corrected SP Name
        //        new { StartDate = start, EndDate = end }, // Passing parameters
        //        commandType: CommandType.StoredProcedure
        //    );
        //}

        public async Task<int> BorrowingTrend(DateTime start, DateTime end)
        {
            return await _dbConn.ExecuteScalarAsync<int>(
                "sp_GetBorrowingTrend",
                new { StartDate = start, EndDate = end }, // Parameters passed here
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<MostBorrowedBookDto>> GetMostBorrowedCustomAsync(DateTime start, DateTime end)
        {
            return await _dbConn.QueryAsync<MostBorrowedBookDto>(
                "sp_GetMostBorrowedBooksCustom",
                new { StartDate = start, EndDate = end },
                commandType: CommandType.StoredProcedure
            );
        }
        public async Task<IEnumerable<TopBorrower>> TopBorrowerAsync(DateTime start, DateTime end)
        {
            return await _dbConn.QueryAsync<TopBorrower>(
                "sp_TopBorrower",
                new { StartDate = start, EndDate = end },
                commandType: CommandType.StoredProcedure
            );
        }

        //public async Task<IEnumerable<UserDto>> TopBorrower() // Needs specific User/Borrower DTO
        //{
        //    return await _dbConn.QueryAsync<UserDto>(
        //        "sp_GetTopBorrowers", // Corrected SP Name
        //        commandType: CommandType.StoredProcedure
        //    );
        //}



        //public async Task<IEnumerable<OverdueDto>> OverdueBooks()
        //{
        //    return await _dbConn.QueryAsync<OverdueDto>(
        //        "sp_GetOverdueBooks", // Corrected SP Name
        //        commandType: CommandType.StoredProcedure
        //);
        //}

    }
}
