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

        public Task<bool> MarkAsReadAsync(int id)
        {
            return _notificationRepository.MarkAsReadAsync(id);
        }

        public Task NotifyBookAddedAsync(string title, string message)
        {
            throw new NotImplementedException();
        }
    }
}
