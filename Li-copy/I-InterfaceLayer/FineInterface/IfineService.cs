using System.Collections.Generic;
using System.Threading.Tasks;
using Li_copy.Models.Fine;

namespace Li_copy.I_InterfaceLayer.FineInterface
{
    public interface IfineService
    {
        Task<Fine?> GetFineByLoanIdAsync(int loanId);
        Task<bool> ProcessPaymentCallbackAsync(int fineId, string transactionId, string callbackJson, string paymentStatus);
        Task<IEnumerable<Fine>> GetAllFinesAsync();
        Task<Fine?> GetFineByIdAsync(int id);
        Task<bool> UpdatePaymentStatusAsync(int fineId, string status); // 🛠️ Added for status orchestration
    }
}