using Li_copy.I_InterfaceLayer.FineInterface;
using Li_copy.Models.Fine;

namespace Li_copy.ServiceLayer.FineServices
{
    public class FineService : IfineService
    {
        private readonly IfineDLL _fineRepository;

        public FineService(IfineDLL fineRepository)
        {
            _fineRepository = fineRepository;
        }

        public async Task<IEnumerable<Fine>> GetAllFinesAsync() => await _fineRepository.GetAllFinesAsync();
        public async Task<Fine?> GetFineByIdAsync(int id) => await _fineRepository.GetFineByIdAsync(id);
        public async Task<Fine?> GetFineByLoanIdAsync(int loanId) => await _fineRepository.GetFineByLoanIdAsync(loanId);

        public async Task<bool> ProcessPaymentCallbackAsync(int fineId, string transactionId, string callbackJson, string paymentStatus)
        {
            var fine = await _fineRepository.GetFineByIdAsync(fineId);
            if (fine == null) return false;

            fine.TransactionId = transactionId;
            fine.PaymentCallbackJson = callbackJson;
            fine.PaymentStatus = paymentStatus;

            if (paymentStatus.ToUpper() == "SUCCESS" || paymentStatus.ToUpper() == "COMPLETED")
            {
                fine.IsPaid = true;
                fine.PaidDate = DateTime.Now;
            }

            return await _fineRepository.UpdateFineAsync(fine);
        }
    }
}
