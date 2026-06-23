using Dapper;
using Li_copy.I_InterfaceLayer.LoanInterface;
using Li_copy.Models.Loans;
using System.Data;

namespace Li_copy.DataLayer.LoanDLL
{
    public class LoanDLL : IloanDLL
    {
        private readonly IDbConnection _dbConnection;

        public LoanDLL(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        //public async Task<Loan?> GetLoanByIdAsync(int id)
        //{
        //    string sql = "SELECT * FROM Loans WHERE Id = @Id";

        //    return await _dbConnection.QueryFirstOrDefaultAsync<Loan>(
        //        sql,
        //        new { Id = id });
        //}

        //public async Task<int> CreateLoanAsync(Loan loan)
        //{
        //    string sql = @"
        //        INSERT INTO Loans
        //        (
        //            BookId,
        //            UserId,
        //            IssueDate,
        //            DueDate,
        //            Status,
        //            BorrowDate,
        //            ReturnDate
        //        )
        //        VALUES
        //        (
        //            @BookId,
        //            @UserId,
        //            @IssueDate,
        //            @DueDate,
        //            @Status,
        //            @BorrowDate,
        //            @ReturnDate
        //        );

        //        SELECT CAST(SCOPE_IDENTITY() AS INT);
        //    ";

        //    return await _dbConnection.ExecuteScalarAsync<int>(
        //        sql,
        //        loan);
        //}

        //public async Task<bool> UpdateLoanAsync(Loan loan)
        //{
        //    string sql = @"
        //        UPDATE Loans
        //        SET
        //            BookId = @BookId,
        //            UserId = @UserId,
        //            IssueDate = @IssueDate,
        //            DueDate = @DueDate,
        //            Status = @Status,
        //            BorrowDate = @BorrowDate,
        //            ReturnDate = @ReturnDate
        //        WHERE Id = @Id";

        //    int rows = await _dbConnection.ExecuteAsync(
        //        sql,
        //        loan);

        //    return rows > 0;
        //}

        //public async Task<bool> DeleteLoanAsync(int id)
        //{
        //    string sql = "DELETE FROM Loans WHERE Id = @Id";

        //    int rows = await _dbConnection.ExecuteAsync(
        //        sql,
        //        new { Id = id });

        //    return rows > 0;
        //}

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            string sql = "SELECT * FROM Loans";
            return await _dbConnection.QueryAsync<Loan>(sql);
        }

        public async Task<Loan?> GetLoanByIdAsync(int id)
        {
            string sql = "SELECT * FROM Loans WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Loan>(sql, new { Id = id });
        }

        public async Task<int> CreateLoanAsync(Loan loan)
        {
            string sql = @"
                INSERT INTO Loans (BookId, UserId, IssueDate, DueDate, Status, BorrowDate, ReturnDate)
                VALUES (@BookId, @UserId, @IssueDate, @DueDate, @Status, @BorrowDate, @ReturnDate);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await _dbConnection.ExecuteScalarAsync<int>(sql, loan);
        }

        public async Task<bool> UpdateLoanAsync(Loan loan)
        {
            string sql = @"
                UPDATE Loans
                SET BookId = @BookId, UserId = @UserId, IssueDate = @IssueDate, DueDate = @DueDate, 
                    Status = @Status, BorrowDate = @BorrowDate, ReturnDate = @ReturnDate
                WHERE Id = @Id";
            int rows = await _dbConnection.ExecuteAsync(sql, loan);
            return rows > 0;
        }

        public async Task<bool> DeleteLoanAsync(int id)
        {
            string sql = "DELETE FROM Loans WHERE Id = @Id";
            int rows = await _dbConnection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
    }


}
