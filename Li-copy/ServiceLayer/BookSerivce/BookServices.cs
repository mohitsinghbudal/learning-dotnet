using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.I_InterfaceLayer.NotificationInterface;
using Li_copy.Models.Book;
using Li_copy.Models.Users;
using Microsoft.AspNetCore.Identity;


namespace Li_copy.ServiceLayer.BookSerivces
{
    public class BookServices : IBookServices
    {
        private readonly IBooksDLL _booksDLL;
        private readonly INotificationService _notificationService;
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
            if (roleId != 3 || roleId !=1)
            {
                throw new UnauthorizedAccessException("Only librarians or admins can add books");
            }
             int newBookId = await _booksDLL.AddBookAsync(book);
            string title = book.Title;
            string message = $"A new book title '{book.Title}' (ID: {newBookId}) has been successfully logged by User ID:{userId}.";
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
    }
}
