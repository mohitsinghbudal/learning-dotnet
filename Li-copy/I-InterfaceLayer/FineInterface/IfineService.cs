using Li_copy.Models.DTO;
using Li_copy.Models.Fine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Li_copy.I_InterfaceLayer.FineInterface
{
    public interface IfineService
    {
        // CRUD Operations
        Task<IEnumerable<Fine>> GetAllFinesAsync();
        Task<Fine?> GetFineByIdAsync(int id);
        Task<IEnumerable<Fine?>> GetFineByUserIdAsync(int userId);
        Task<Fine?> GetFineByLoanIdAsync(int userId);
        Task<bool> UpdatePaymentStatusAsync(int fineId, string status);

        // Payment Flow
        Task<EsewaPaymentResponseDTO?> PayFineAsync(EsewaPaymentRequestDTO request);

        // Callbacks & Verification
        Task<bool> ProcessPaymentCallbackAsync(int fineId, string transactionId, string callbackJson, string paymentStatus);
        Task<bool> ProcessPaymentCallbackAsync(PaymentCallbackDTO dto);
        Task<bool> ProcessEsewaSuccessAsync(string encodedData);
    }
}