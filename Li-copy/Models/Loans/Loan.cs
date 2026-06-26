namespace Li_copy.Models.Loans
{
    public class Loan
    {

        public int Id { get; set; }

        public int BookId { get; set; }


        public int UserId { get; set; }

        public DateTime? IssuedDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public int? VerifiedByUserId { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public DateTime? BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public int? IssuedByUserId { get; set; }

    }
}
