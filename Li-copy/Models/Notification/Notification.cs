namespace Li_copy.Models.Notification
{
    public class Notification
    {
        
            public int Id { get; set; }

            public string Title { get; set; } = string.Empty;

            public string Message { get; set; } = string.Empty;

            public string Type { get; set; } = string.Empty;

            public bool IsRead { get; set; }

            public DateTime CreatedAt { get; set; }

            public int? CreatedBy { get; set; }

            public string TargetRole { get; set; } = "ADMIN";
        
    }

}
