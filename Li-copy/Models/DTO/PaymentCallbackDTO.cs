namespace Li_copy.Models.DTO
{
    public class PaymentCallbackDTO
    {
        public string TransactionId { get; set; } = string.Empty;
        public string RawCallbackJson { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
    }
}