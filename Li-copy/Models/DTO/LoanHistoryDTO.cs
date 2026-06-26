namespace Li_copy.Models.Book
{
    public class LoanHistoryDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int UserId { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; } // Nullable because it matches (datetime, null)
        public int? IssuedByUserId { get; set; }  // Nullable because it matches (int, null)
    }
}