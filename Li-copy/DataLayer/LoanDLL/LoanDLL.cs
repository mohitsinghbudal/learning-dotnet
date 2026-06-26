using Dapper;
using Li_copy.I_InterfaceLayer.LoanInterface;
using Li_copy.Models.Book;
using Li_copy.Models.Loans;
using Microsoft.Data.SqlClient;
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

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            string sql = "SELECT * FROM Loans";
            return await _dbConnection.QueryAsync<Loan>(sql);
        }

        public async Task<IEnumerable<LoanHistoryDto>> GetPersonalBorrowHistoryAsync(int userId)
        {
            string sql = @"
                SELECT 
                    l.Id,
                    l.BookId,
                    b.Title,
                    b.Author,
                    l.UserId,
                    l.IssueDate,
                    l.DueDate,
                    l.Status,
                    l.BorrowDate,
                    l.ReturnDate,
                    l.IssuedByUserId,
                    l.VerifiedByUserId
                FROM Loans l
                INNER JOIN Books b ON l.BookId = b.Id
                WHERE l.UserId = @userId
                ORDER BY l.BorrowDate DESC";

            return await _dbConnection.QueryAsync<LoanHistoryDto>(sql, new { userId });
        }

        public async Task<IEnumerable<LoanHistoryDto>> GetAllBorrowedBooksAsync()
        {
            string sql = @"
                SELECT 
                    l.Id,
                    l.BookId,
                    b.Title,
                    b.Author,
                    l.UserId,
                    l.IssueDate,
                    l.DueDate,
                    l.Status,
                    l.BorrowDate,
                    l.ReturnDate,
                    l.IssuedByUserId,
                    l.VerifiedByUserId
                FROM Loans l
                INNER JOIN Books b ON l.BookId = b.Id
                ORDER BY l.BorrowDate DESC";

            return await _dbConnection.QueryAsync<LoanHistoryDto>(sql);
        }

        public async Task<LoanDTO?> GetLoanByIdAsync(int id)
        {
            string sql = @"SELECT * FROM Loans WHERE userId = @id";
            return await _dbConnection.QueryFirstOrDefaultAsync<LoanDTO>(sql, new { id });
        }

        public async Task<int> CreateLoanAsync(Loan loan)
        {
            // Pass only what the student provides on initial request submission
            string sql = @"
        INSERT INTO Loans (BookId, UserId, DueDate, Status, BorrowDate)
        VALUES (@BookId, @UserId, @DueDate, @Status, @BorrowDate);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await _dbConnection.ExecuteScalarAsync<int>(sql, loan);
        }

        public async Task<bool> UpdateLoanAsync(Loan loan)
        {
            string sql = @"
                UPDATE Loans
                SET BookId = @BookId, 
                    UserId = @UserId, 
                    IssueDate = @IssuedDate, 
                    DueDate = @DueDate, 
                    Status = @Status, 
                    BorrowDate = @BorrowDate, 
                    ReturnDate = @ReturnDate,
                    IssuedByUserId = @IssuedByUserId,
                    VerifiedByUserId = @VerifiedByUserId
                WHERE Id = @Id";
            int rows = await _dbConnection.ExecuteAsync(sql, loan);
            return rows > 0;
        }

        public async Task<bool> VerifyBorrowAsync(int loanId, int issuedByUserId)
        {
            string sql = @"
                UPDATE Loans
                SET 
                    Status = 'Borrowed',
                    IssuedByUserId = @IssuedByUserId,
                    IssueDate = GETDATE() 
                WHERE Id = @LoanId";

            int rowsAffected = await _dbConnection.ExecuteAsync(sql, new
            {
                LoanId = loanId,
                IssuedByUserId = issuedByUserId
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteLoanAsync(int id)
        {
            string sql = "DELETE FROM Loans WHERE Id = @Id";
            int rows = await _dbConnection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        async Task<Loan?> IloanDLL.GetLoanByIdAsync(int id)
        {
            string sql = "SELECT * FROM Loans WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<Loan?>(sql, new { Id = id });
        }

        public async Task<IEnumerable<LoanDTO?>> GetUserLoanAsync(int userId)
        {
            string sql = @"
                SELECT 
                    L.Id,    
                    L.UserId,    
                    L.BookId,    
                    B.Title AS BookTitle,    
                    B.Author,    
                    L.BorrowDate,    
                    L.DueDate,
                    L.ReturnDate,    
                    L.Status,
                    L.IssueDate,
                    L.IssuedByUserId,
                    L.VerifiedByUserId
                FROM Loans L
                INNER JOIN Books B ON L.BookId = B.Id
                WHERE L.UserId = @UserId";

            return await _dbConnection.QueryAsync<LoanDTO>(sql, new { UserId = userId });
        }

        public async Task UpdateLoanAsync(LoanDTO loan)
        {
            string sql = @"
                UPDATE Loans 
                SET ReturnDate = @ReturnDate, 
                    Status = @Status 
                WHERE Id = @Id";

            await _dbConnection.ExecuteAsync(sql, loan);
        }

        // Student sets the record to 'Pending Return'
        public async Task<bool> ReturnBookAsync(int loanId)
        {
            string sql = @"
                UPDATE Loans
                SET Status = 'Pending Return'
                WHERE Id = @LoanId AND Status = 'Borrowed'";

            int rowsAffected = await _dbConnection.ExecuteAsync(sql, new { LoanId = loanId });
            return rowsAffected > 0;
        }

        // Teacher finalizes return via clean Transaction scoping
        public async Task<bool> UpdateAndReleaseInventoryAsync(Loan loan)
        {
            if (_dbConnection.State == ConnectionState.Closed)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                // 1. Update Loan confirmation fields 
                string updateLoanSql = @"
                    UPDATE Loans 
                    SET Status = @Status, 
                        VerifiedByUserId = @VerifiedByUserId, 
                        ReturnDate = GETDATE() 
                    WHERE Id = @Id";

                // 🛠️ FIXED: Anonymous type parameters now correctly align with the tracking variable signatures (@VerifiedByUserId)
                int loanRows = await _dbConnection.ExecuteAsync(updateLoanSql, new
                {
                    Status = loan.Status,
                    VerifiedByUserId = loan.VerifiedByUserId,
                    Id = loan.Id
                }, transaction);

                // 2. Increment Available copies inside Book schema 
                string updateBookSql = @"
                    UPDATE Books 
                    SET AvailableCopies = AvailableCopies + 1 
                    WHERE Id = @BookId";

                int bookRows = await _dbConnection.ExecuteAsync(updateBookSql, new { BookId = loan.BookId }, transaction);

                transaction.Commit();
                return loanRows > 0 && bookRows > 0;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}