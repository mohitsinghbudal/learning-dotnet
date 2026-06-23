namespace Li_copy.Models.Book
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public int CategoryId { get; set; }

        public string ISBN { get; set; } = string.Empty;

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        public int YearPublished { get; set; }

        // FK instead of string
        public int AddedByUserId { get; set; }

        // older code used CreatedByUserId in controllers; provide a compatibility wrapper
        public int CreatedByUserId
        {
            get => AddedByUserId;
            set => AddedByUserId = value;
        }

        public int? ApprovedByUserId { get; set; }

        public bool IsApproved { get; set; } = false;
    }
}