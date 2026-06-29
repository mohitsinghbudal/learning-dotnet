namespace Li_copy.Models.DTO
{
    public class EsewaSuccessResponseDTO
    {
        public string transaction_code { get; set; }

        public string status { get; set; }

        public decimal total_amount { get; set; }

        public string transaction_uuid { get; set; }

        public string product_code { get; set; }

        public string signature { get; set; }

        public string signed_field_names { get; set; }
    }
}
