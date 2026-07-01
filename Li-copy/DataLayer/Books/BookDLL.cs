using Dapper;
using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.Models.Book;
using Li_copy.Models.Category;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.Common;

namespace Li_copy.DataLayer.Books
{
    public class BookDLL : IBooksDLL
    {
        private readonly IDbConnection _dbConn;

        public BookDLL(IDbConnection dbconn)
        {
            _dbConn = dbconn;
        }


        public async Task<int> GetCount()
        {
            string sql = "SELECT SUM(AvailableCopies) FROM Books";
            return await _dbConn.ExecuteScalarAsync<int>(sql);
        }
        public async Task<IEnumerable<Book>> GetVerifiedBookAsync(int offset, int pagesize)
        {
            string sql = "SELECT * FROM Books WHERE IsApproved = 1 ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            return await _dbConn.QueryAsync<Book>(sql, new {Offset = offset, PageSize = pagesize});
        }
        public async Task<IEnumerable<Book>> GetBooksAsync(int offset, int pagesize)
        {
            string sql = "SELECT * FROM Books ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            return await _dbConn.QueryAsync<Book>(sql, new {Offset = offset, PageSize = pagesize});
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
    ApprovedByUserId,
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
    @ApprovedByUserId,
    0
);

SELECT CAST(SCOPE_IDENTITY() as int);";

            return await _dbConn.ExecuteScalarAsync<int>(sql, book);
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
    AddedBy = @adminId
WHERE Id = @bookId
  AND IsApproved = 0";

            int rowsAffected = await _dbConn.ExecuteAsync(
                sql,
                new { bookId, adminId }
            );

            return rowsAffected;
        }

        // NEW: Creates a new pending borrow ledger request for students
        public async Task<int> CreateBorrowRequestAsync(int bookId, int studentId)
        {
            string checkSql = @"
        SELECT AvailableCopies
        FROM Books
        WHERE Id = @bookId
          AND IsApproved = 1";

            int? availableCopies = await _dbConn.ExecuteScalarAsync<int?>(
                checkSql,
                new { bookId });

            if (availableCopies == null || availableCopies <= 0)
            {
                throw new InvalidOperationException(
                    "This book is currently out of stock or unverified.");
            }

            string sql = @"
        INSERT INTO Loans
        (
            BookId,
            UserId,
            ReturnDate,
            DueDate,            
            Status,
            BorrowDate
        )
        VALUES
        (
            @bookId,
            @studentId,
            DATEADD(DAY, 14, GETDATE()),
            DATEADD(DAY, 14, GETDATE()),
            'Pending',
            GETDATE()
        );

        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await _dbConn.ExecuteScalarAsync<int>(
                sql,
                new { bookId, studentId });
        }

        // NEW: Processes approval actions and safely updates remaining book metrics
        public async Task<bool> ApproveBorrowRequestAsync(int LoanId, int librarianId)
        {
            Console.WriteLine("reached dll");
            // Open connection if closed to cleanly handle explicit transactions
            if (_dbConn.State == ConnectionState.Closed) _dbConn.Open();

            using (var transaction = _dbConn.BeginTransaction())
            {
                try
                {
                    // 1. Fetch the BookId associated with this specific borrow ledger transaction
                    string getBookIdSql = "SELECT BookId FROM Loans WHERE Id = @LoanId AND Status = 'Pending'";
                    int bookId = await _dbConn.ExecuteScalarAsync<int>(getBookIdSql, new { LoanId }, transaction);

                    if (bookId <= 0) return false; // Request doesn't exist or isn't in a 'Pending' state

                    // 2. Update status of the borrow records index flag to 'Approved'
                    string updateRequestSql = @"
UPDATE Loans
SET Status = 'Approved', 
IssuedByUserId = @librarianId, 
IssueDate = GETDATE() 
WHERE Id = @LoanId";

                    await _dbConn.ExecuteAsync(updateRequestSql, new { LoanId, librarianId }, transaction);

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
        public async Task IncrementAvailableCopiesAsync(int bookId)
        {
            string sql = @"
        UPDATE Books 
        SET AvailableCopies = AvailableCopies + 1 
        WHERE Id = @BookId AND AvailableCopies < TotalCopies";

            await _dbConn.ExecuteAsync(sql, new { BookId = bookId });
        }

        public async Task<IEnumerable<Book>> SearchBookAsync(string? title, string? author, string? category)
        {
            // Using a StringBuilder or clean spacing makes SQL manipulation safer/readable
            var sql = @"SELECT b.* FROM Books b 
                INNER JOIN Categories c ON b.CategoryId = c.Id 
                WHERE 1 = 1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(title))
            {
                sql += " AND b.Title LIKE @Title";
                parameters.Add("Title", $"%{title}%");
            }

            if (!string.IsNullOrWhiteSpace(author))
            {
                sql += " AND b.Author LIKE @Author";
                parameters.Add("Author", $"%{author}%");
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                sql += " AND c.Name LIKE @Category";
                parameters.Add("Category", $"%{category}%");
            }

            // FIX: Pass 'parameters' directly instead of wrapping it in an anonymous object
            return await _dbConn.QueryAsync<Book>(sql, parameters);
        }


        public Task DecrementAvailableCopiesAsync(int bookId)
        {
            throw new NotImplementedException();
        }
    }
}