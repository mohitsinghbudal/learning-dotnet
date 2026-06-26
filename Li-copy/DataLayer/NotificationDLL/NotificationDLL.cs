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

        public async Task<int> CreateNotificationAsync(string title, string message, string targetRole, string notificationType, int userId)
        {
            // 1. Fixed the duplicate TargetRole
            // 2. Included the required Title column
            // 3. Normalized parameters to match the Dapper anonymous object properties
            string sql = @"
        INSERT INTO Notifications (
            Title,
            Message, 
            Type, 
            IsRead, 
            CreatedAt,
            CreatedBy,
            TargetRole
        )
        VALUES (
            @title,
            @message, 
            @notificationType, 
            0, 
            GETDATE(),
            @userId,
            @targetRole
        );
        
        SELECT CAST(SCOPE_IDENTITY() as int);";

            // Ensure all variables referenced via '@' in the query match the object keys below
            var result = await _db.ExecuteScalarAsync<int>(sql, new
            {
                title,
                message,
                notificationType,
                userId,
                targetRole
            });

            return result;
        }
    }
}