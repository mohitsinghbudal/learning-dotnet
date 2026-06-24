using Dapper;
using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.Models.Book;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Li_copy.DataLayer.Books
{
    public class BookDLL : IBooksDLL
    {
        private readonly IDbConnection _dbConn;

        public BookDLL(IDbConnection dbconn)
        {
            _dbConn = dbconn;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            string sql = "SELECT * FROM Books";
            return await _dbConn.QueryAsync<Book>(sql);
        }

        public async Task<int> AddBookAsync(Book book)
        {
            string sql = @"
INSERT INTO Books 
(
    Title,
    Author,
    CategoryId,
    ISBN,
    TotalCopies,
    AvailableCopies,
    YearPublished,
    AddedByUserId,
    IsApproved
)
VALUES
(
    @Title,
    @Author,
    @CategoryId,
    @ISBN,
    @TotalCopies,
    @AvailableCopies,
    @YearPublished,
    @CreatedByUserId, -- Note: Mapped to your model property name from the controller
    0
);
SELECT CAST(SCOPE_IDENTITY() as int);";

            var response = await _dbConn.ExecuteScalarAsync<int>(sql, book);
            return response;
        }

        public async Task<IEnumerable<Book>> GetUnverifiedBooksAsync()
        {
            string sql = @"
SELECT *
FROM Books
WHERE IsApproved = 0";

            return await _dbConn.QueryAsync<Book>(sql);
        }

        public async Task<int> VerifyBookAsync(int bookId, int adminId)
        {
            string sql = @"
UPDATE Books
SET IsApproved = 1,
    ApprovedByUserId = @adminId
WHERE Id = @bookId";

            return await _dbConn.ExecuteAsync(sql, new { bookId, adminId });
        }

        // NEW: Creates a new pending borrow ledger request for students
        public async Task<int> CreateBorrowRequestAsync(int bookId, int studentId)
        {
            // First check if copies are actually available before creating a request
            string checkSql = "SELECT AvailableCopies FROM Books WHERE Id = @bookId AND IsApproved = 1";
            int availableCopies = await _dbConn.ExecuteScalarAsync<int>(checkSql, new { bookId });

            if (availableCopies <= 0)
            {
                throw new InvalidOperationException("This book is currently out of stock or unverified.");
            }

            string sql = @"
INSERT INTO BorrowRequests (BookId, StudentId, Status, RequestDate)
VALUES (@bookId, @studentId, 'Pending', GETDATE());
SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _dbConn.ExecuteScalarAsync<int>(sql, new { bookId, studentId });
        }

        // NEW: Processes approval actions and safely updates remaining book metrics
        public async Task<bool> ApproveBorrowRequestAsync(int requestId, int librarianId)
        {
            // Open connection if closed to cleanly handle explicit transactions
            if (_dbConn.State == ConnectionState.Closed) _dbConn.Open();

            using (var transaction = _dbConn.BeginTransaction())
            {
                try
                {
                    // 1. Fetch the BookId associated with this specific borrow ledger transaction
                    string getBookIdSql = "SELECT BookId FROM BorrowRequests WHERE Id = @requestId AND Status = 'Pending'";
                    int bookId = await _dbConn.ExecuteScalarAsync<int>(getBookIdSql, new { requestId }, transaction);

                    if (bookId <= 0) return false; // Request doesn't exist or isn't in a 'Pending' state

                    // 2. Update status of the borrow records index flag to 'Approved'
                    string updateRequestSql = @"
UPDATE BorrowRequests 
SET Status = 'Approved', ApprovedByUserId = @librarianId, ApprovalDate = GETDATE() 
WHERE Id = @requestId";

                    await _dbConn.ExecuteAsync(updateRequestSql, new { requestId, librarianId }, transaction);

                    // 3. Decrement the availability counter safely on the primary collection records matrix
                    string decrementInventorySql = @"
UPDATE Books 
SET AvailableCopies = AvailableCopies - 1 
WHERE Id = @bookId AND AvailableCopies > 0";

                    int rowsAffected = await _dbConn.ExecuteAsync(decrementInventorySql, new { bookId }, transaction);

                    if (rowsAffected == 0)
                    {
                        // Rollback transaction safely if a book runout occurred mid-flight
                        transaction.Rollback();
                        throw new InvalidOperationException("No available copies left to fulfill approval request.");
                    }

                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}