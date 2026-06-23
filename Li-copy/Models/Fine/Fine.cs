namespace Li_copy.Models.Fine
{
    public class Fine
    {
        public int Id { get; set; }
        public int LoanId { get; set; }

        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }

        public DateTime? PaidDate{ get; set; }

        public string? TransactionId { get; set; }
        public string? PaymentStatus { get; set; }

        public string? PaymentRequestJson { get; set; }

        public string? PaymentInitResponseJson { get; set; }

        public string? PaymentCallbackJson { get; set; }

        public string? PaymentFinalResponseJson { get; set; }
    }
}
