namespace Li_copy.Models.DTO
{
    public class EsewaTransactionStatusDTO
    {
        public string product_code { get; set; }

        public string transaction_uuid { get; set; }

        public decimal total_amount { get; set; }

        public string status { get; set; }

        public string ref_id { get; set; }
    }
}
