using Microsoft.VisualBasic;

namespace Li_copy.Models.Loans
{
    public class LoanDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }

        // From joined Books table
        public string BookTitle { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        // From Loans table
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;

        // These fields are nullable in your database, so they must be nullable here
        public DateTime? ReturnDate { get; set; }
        public DateTime? IssuedDate { get; set; }
        public int? IssuedByUserId { get; set; }
    }
}
