using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.I_InterfaceLayer.NotificationInterface;
using Li_copy.Models.Book;

namespace Li_copy.ServiceLayer.BookSerivces
{
    public class BookServices : IBookServices
    {
        private readonly IBooksDLL _booksDLL;
        private readonly INotificationService _notificationService; // Fix: Use Service, not DLL directly

        public BookServices(IBooksDLL booksDLL, INotificationService notificationService)
        {
            _booksDLL = booksDLL;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Book>> GetBooksAsync()
        {
            return await _booksDLL.GetBooksAsync();
        }

        public async Task<int> AddBookAsync(Book book, int roleId, int userId)
        {
            // Fix: Changed '||' to '&&' so authorized users are not blocked
            if (roleId != 3 && roleId != 1)
            {
                throw new UnauthorizedAccessException("Only librarians or admins can add books");
            }

            int newBookId = await _booksDLL.AddBookAsync(book);
            string title = book.Title;

            await _notificationService.NotifyBookAddedAsync(title, userId);

            return newBookId;
        }

        public async Task<bool> VerifyBookAsync(int bookId, int roleId, int adminId)
        {
            if (roleId != 1)
            {
                return false;
            }
            return await _booksDLL.VerifyBookAsync(bookId, adminId) > 0;
        }

        public async Task<bool> CreateBorrowRequestAsync(int bookId, int roleId, int studentId)
        {
            if (roleId != 2)
            {
                throw new UnauthorizedAccessException("Only students can request to borrow books.");
            }

            int requestId = await _booksDLL.CreateBorrowRequestAsync(bookId, studentId);

            if (requestId > 0)
            {
                await _notificationService.NotifyBorrowRequestedAsync(requestId, studentId, bookId);
                return true;
            }

            return false;
        }

        public async Task<bool> ApproveBorrowRequestAsync(int requestId, int roleId, int librarianId)
        {
            if (roleId != 3 && roleId != 1)
            {
                return false;
            }

            return await _booksDLL.ApproveBorrowRequestAsync(requestId, librarianId);
        }
    }
}