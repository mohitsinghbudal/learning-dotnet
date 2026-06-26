using Li_copy.I_InterfaceLayer.NotificationInterface;
using Li_copy.Models.Notification;

namespace Li_copy.ServiceLayer.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationDLL _notificationRepository;

        public NotificationService(INotificationDLL notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public Task<int> CreateAsync(Notification notification)
        {
            return _notificationRepository.CreateAsync(notification);
        }

        public async Task NotifyBookAddedAsync(string bookName, int librarianId)
        {
            var notification = new Notification
            {
                Title = "New Book Added",
                Message = $"Book Name : '{bookName}' was added by librarian : {librarianId}",
                Type = "BOOK_ADDED",
                CreatedBy = librarianId,
                TargetRole = "ADMIN",
                IsRead = false
            };

            await _notificationRepository.CreateAsync(notification);
        }

        public Task<IEnumerable<Notification>> GetAdminNotificationsAsync()
        {
            return _notificationRepository.GetAdminNotificationsAsync();
        }

        // NEW: Pulls librarian alerts from the data layer
        public Task<IEnumerable<Notification>> GetLibrarianNotificationsAsync()
        {
            return _notificationRepository.GetLibrarianNotificationsAsync();
        }

        public Task<bool> MarkAsReadAsync(int id)
        {
            return _notificationRepository.MarkAsReadAsync(id);
        }

        public async Task NotifyBorrowRequestedAsync(int requestId, int studentId, int bookId)
        {
            // Added a title since the database schema marks it as NOT NULL
            string title = "New Borrow Request";
            string message = $"Student (ID: {studentId}) requested to borrow Book ID: {bookId}. (Request ID: {requestId})";

            Console.WriteLine(message);

            // Matching your database schema expectations for TargetRole (string representation)
            string targetRole = "LIBRARIAN";
            string notificationType = "BORROW_REQ";

            // Pass all 5 arguments matching the repository signature:
            // CreateNotificationAsync(string title, string message, string targetRole, string notificationType, int userId)
            await _notificationRepository.CreateNotificationAsync(title, message, targetRole, notificationType, studentId);
        }

        // NEW: Computes unread alerts for Admins directly in the service layer
        public async Task<int> GetAdminUnreadCountAsync()
        {
            var all = await _notificationRepository.GetAdminNotificationsAsync();
            return all.Count(x => !x.IsRead);
        }

        // NEW: Computes unread alerts for Librarians directly in the service layer
        public async Task<int> GetLibrarianUnreadCountAsync()
        {
            var all = await _notificationRepository.GetLibrarianNotificationsAsync();
            return all.Count(x => !x.IsRead);
        }
    }
}