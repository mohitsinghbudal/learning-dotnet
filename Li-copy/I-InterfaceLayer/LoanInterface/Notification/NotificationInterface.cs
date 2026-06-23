
using Li_copy.Models.Notification;

namespace Li_copy.I_InterfaceLayer.NotificationInterface
{
    public interface INotificationDLL
    {
        Task<int> CreateAsync(Notification notification);

        Task<IEnumerable<Notification>> GetAdminNotificationsAsync();
        Task<bool> MarkAsReadAsync(int id);
    }


    public interface INotificationService
    {
        //Task<int> CreateAsync(Li_copy.Models.Notification.Notification notification);
        Task NotifyBookAddedAsync(string bookName, int librarianId);
        Task<int> CreateAsync(Notification notification);
        Task NotifyBookAddedAsync(string title, string message);
        Task<IEnumerable<Notification>> GetAdminNotificationsAsync();
        Task<bool> MarkAsReadAsync(int id);
    }
}
