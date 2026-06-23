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

        public async Task<bool> MarkAsReadAsync(int id)
        {
            string sql = "UPDATE Notifications SET IsRead=1 WHERE Id=@Id";
            return await _db.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}
