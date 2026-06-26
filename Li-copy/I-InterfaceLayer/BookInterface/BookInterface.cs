using Li_copy.Models.Book;

namespace Li_copy.I_InterfaceLayer.BookInterface
{
    public interface IBooksDLL
    {
        Task<IEnumerable<Book>> GetBooksAsync();
        Task<int> AddBookAsync(Book book);
        Task<IEnumerable<Book>> GetUnverifiedBooksAsync();
        Task<int> VerifyBookAsync(int bookId, int adminId);
        Task<int> CreateBorrowRequestAsync(int bookId, int studentId);
        Task<bool> ApproveBorrowRequestAsync(int BookId, int librarianId);
        Task<IEnumerable<Book>> GetVerifiedBookAsync();
        Task IncrementAvailableCopiesAsync(int bookId);
        Task DecrementAvailableCopiesAsync(int bookId);

    }

        public interface IBookServices
        {
            Task<IEnumerable<Book>> GetBooksAsync();
            Task<int> AddBookAsync(Book book, int roleId, int UserId);
            Task<bool> VerifyBookAsync(int bookId, int roleId, int adminId);

            Task<bool> CreateBorrowRequestAsync(int bookId, int roleId, int studentId);
            Task<bool> ApproveBorrowRequestAsync(int studentId, int roleId, int librarianId);

            Task<IEnumerable<Book>> GetVerifiedBookAsync();



        }
    }

