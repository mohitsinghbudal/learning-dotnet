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
    @AddedByUserId,
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
    }
}



