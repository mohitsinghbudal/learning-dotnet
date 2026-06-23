using Li_copy.Models.Book;

namespace Li_copy.I_InterfaceLayer.BookInterface
{
    public interface IBooksDLL
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<int> AddBookAsync(Book book);
        Task<IEnumerable<Book>> GetUnverifiedBooksAsync();
        Task<int> VerifyBookAsync(int bookId, int adminId);
    }

    public interface IBookServices
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<int> AddBookAsync(Book book, int roleId, int UserId);
        Task<bool> VerifyBookAsync(int bookId, int roleId, int adminId);
    }
}
