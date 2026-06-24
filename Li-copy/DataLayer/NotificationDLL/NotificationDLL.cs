using Dapper;
using Li_copy.I_InterfaceLayer.NotificationInterface;
using Li_copy.Models.Notification;
using System.Data;

namespace Li_copy.DataLayer.NotificationDLL
{
    public class NotificationDLL : INotificationDLL
    {
        private readonly IDbConnection _db;

        public NotificationDLL(IDbConnection db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(Notification notification)
        {
            string sql = @"
                INSERT INTO Notifications
                (Title, Message, Type, IsRead, CreatedAt, CreatedBy, TargetRole)
                VALUES
                (@Title, @Message, @Type, @IsRead, GETDATE(), @CreatedBy, @TargetRole);

                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            return await _db.ExecuteScalarAsync<int>(sql, notification);
        }

        public async Task<IEnumerable<Notification>> GetAdminNotificationsAsync()
        {
            string sql = "SELECT * FROM Notifications WHERE TargetRole='ADMIN' ORDER BY CreatedAt DESC";
            return await _db.QueryAsync<Notification>(sql);
        }

        // NEW: Added to support fetching librarian-specific alerts
        public async Task<IEnumerable<Notification>> GetLibrarianNotificationsAsync()
        {
            // Checks for both the string identifier 'LIBRARIAN' or role id '3' depending on your seed data approach
            string sql = "SELECT * FROM Notifications WHERE TargetRole='LIBRARIAN' OR TargetRole='3' ORDER BY CreatedAt DESC";
            return await _db.QueryAsync<Notification>(sql);
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            string sql = "UPDATE Notifications SET IsRead=1 WHERE Id=@Id";
            return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<int> CreateNotificationAsync(string message, int targetRoleId, string notificationType)
        {
            string sql = @"
INSERT INTO Notifications 
(
    Message, 
    TargetRoleId, 
    Type, 
    IsRead, 
    CreatedDate,
    TargetRole
)
VALUES 
(
    @message, 
    @targetRoleId, 
    @notificationType, 
    0, 
    GETDATE(),
    CASE WHEN @targetRoleId = 1 THEN 'ADMIN' WHEN @targetRoleId = 3 THEN 'LIBRARIAN' ELSE 'USER' END
);
SELECT CAST(SCOPE_IDENTITY() as int);";

            var result = await _db.ExecuteScalarAsync<int>(sql, new
            {
                message,
                targetRoleId,
                notificationType
            });

            return result;
        }
    }
}