using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using Li_copy.DataLayer.FineDLL;
using Li_copy.I_InterfaceLayer.FineInterface;
using Li_copy.Models.DTO;
using Li_copy.Models.Fine;

namespace Li_copy.ServiceLayer.FineServices
{
    public class FineService : IfineService
    {
        private readonly IfineDLL _fineRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public FineService(
            IfineDLL fineRepository,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _fineRepository = fineRepository;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        // --- Helper for Security ---
        private string GenerateSignature(string message, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToBase64String(hash);
        }

        // --- CRUD / Standard Methods ---
        public async Task<IEnumerable<Fine>> GetAllFinesAsync() => await _fineRepository.GetAllFinesAsync();

        public async Task<Fine?> GetFineByIdAsync(int id) => await _fineRepository.GetFineByIdAsync(id);

        public async Task<IEnumerable<Fine?>> GetFineByUserIdAsync(int userId) => await _fineRepository.GetFineByUserIdAsync(userId);

        public async Task<Fine?> GetFineByLoanIdAsync(int userId) => await _fineRepository.GetFineByLoanIdAsync(userId);

        public async Task<bool> UpdatePaymentStatusAsync(int fineId, string status)
        {
            var fine = await _fineRepository.GetFineByIdAsync(fineId);
            if (fine == null) return false;
            fine.PaymentStatus = status;
            return await _fineRepository.UpdateFineAsync(fine);
        }

        // --- Payment Initiation ---
        public async Task<EsewaPaymentResponseDTO?> PayFineAsync(EsewaPaymentRequestDTO request)
        {
            var fine = await _fineRepository.GetFineByIdAsync(request.id);
            if (fine == null || fine.IsPaid) return null;

            Console.WriteLine("service to the fine is doing here");

            var amountStr = (fine.Amount).ToString("0.00");
            var transactionUuid = $"FINE-{fine.Id}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            var merchantId = _configuration["Esewa:MerchantId"];
            var secretKey = _configuration["Esewa:SecretKey"];
            var esewaUrl = _configuration["Esewa:BaseUrl"];
            var successUrl = _configuration["Esewa:SuccessUrl"];
            var failureUrl = _configuration["Esewa:FailureUrl"];

            Console.WriteLine(merchantId);


            string message = $"total_amount={amountStr},transaction_uuid={transactionUuid},product_code={merchantId}";
            string signature = GenerateSignature(message, secretKey);

            
            
               
                
                
            var response = new EsewaPaymentResponseDTO
            {
                esewaUrl = esewaUrl,

                amount = amountStr,
                tax_amount = "0",
                total_amount = amountStr,

                transaction_uuid = transactionUuid,
                product_code = merchantId,

                product_service_charge = "0",
                product_delivery_charge = "0",

                success_url = successUrl,
                failure_url = failureUrl,
                //secretKey = secretKey,

                signed_field_names = "total_amount,transaction_uuid,product_code",

                signature = signature
            };

            await _fineRepository.UpdatePaymentInitializationAsync(
                fine.Id, transactionUuid, "{}", JsonSerializer.Serialize(response));

            return response;
        }

        // --- Callback Processing (Webhook) ---
        public async Task<bool> ProcessPaymentCallbackAsync(PaymentCallbackDTO dto)
        {
            var fine = await _fineRepository.GetFineByTransactionIdAsync(dto.TransactionId);
            if (fine == null) return false;

            fine.PaymentCallbackJson = dto.RawCallbackJson;
            fine.PaymentStatus = dto.PaymentStatus;

            if (dto.PaymentStatus.ToUpper() == "COMPLETE" || dto.PaymentStatus.ToUpper() == "SUCCESS")
            {
                fine.IsPaid = true;
                fine.PaidDate = DateTime.Now;
            }

            return await _fineRepository.UpdateFineAsync(fine);
        }

        // --- Overload for Manual/System updates ---
        public async Task<bool> ProcessPaymentCallbackAsync(int fineId, string transactionId, string callbackJson, string paymentStatus)
        {
            var fine = await _fineRepository.GetFineByIdAsync(fineId);
            if (fine == null) return false;

            fine.TransactionId = transactionId;
            fine.PaymentCallbackJson = callbackJson;
            fine.PaymentStatus = paymentStatus;

            if (paymentStatus.ToUpper() == "COMPLETE" || paymentStatus.ToUpper() == "SUCCESS")
            {
                fine.IsPaid = true;
                fine.PaidDate = DateTime.Now;
            }

            return await _fineRepository.UpdateFineAsync(fine);
        }

        // --- eSewa Success Redirect Verification ---
        public async Task<bool> ProcessEsewaSuccessAsync(string encodedData)
        {
            Console.WriteLine("sucess esewa");
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(encodedData));
            var payment = JsonSerializer.Deserialize<EsewaSuccessResponseDTO>(json);
            if (payment == null) return false;

            string url = $"https://rc-epay.esewa.com.np/api/epay/transaction/status/?product_code={payment.product_code}&total_amount={payment.total_amount}&transaction_uuid={payment.transaction_uuid}";

            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return false;

            var body = await response.Content.ReadAsStringAsync();
            var status = JsonSerializer.Deserialize<EsewaTransactionStatusDTO>(body);

            if (status?.status == "COMPLETE")
            {
                return await _fineRepository.MarkFinePaidAsync(payment.transaction_uuid, json, body);
            }
            return false;
        }
    }
}