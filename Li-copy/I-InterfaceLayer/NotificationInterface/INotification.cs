using Li_copy.Models.Notification;

namespace Li_copy.I_InterfaceLayer.NotificationInterface
{
    public interface INotificationDLL
    {
        Task<int> CreateAsync(Notification notification);
        Task<IEnumerable<Notification>> GetAdminNotificationsAsync();

        // ADDED: Needed by the repository/DLL to query librarian data rows
        Task<IEnumerable<Notification>> GetLibrarianNotificationsAsync();

        Task<bool> MarkAsReadAsync(int id);
        Task<int> CreateNotificationAsync(string title, string message, string targetRole, string notificationType, int userId);
    }

    public interface INotificationService
    {
        Task<int> CreateAsync(Notification notification);
        Task<IEnumerable<Notification>> GetAdminNotificationsAsync();

        // ADDED: Needed by the controller to route librarian alerts
        Task<IEnumerable<Notification>> GetLibrarianNotificationsAsync();

        Task<bool> MarkAsReadAsync(int id);
        Task NotifyBookAddedAsync(string bookName, int librarianId);
        Task NotifyBorrowRequestedAsync(int requestId, int studentId, int bookId);

        // ADDED: Needed by the controller to pull dashboard counts
        Task<int> GetAdminUnreadCountAsync();
        Task<int> GetLibrarianUnreadCountAsync();
    }
}