namespace Li_copy.I_InterfaceLayer.FineInterface
{
    using Li_copy.Models.Fine;

    public interface IfineService
    {
        Task<Fine?> GetFineByLoanIdAsync(int loanId);
        Task<bool> ProcessPaymentCallbackAsync(int fineId, string transactionId, string callbackJson, string paymentStatus);
        Task<IEnumerable<Fine>> GetAllFinesAsync();
        Task<Fine?> GetFineByIdAsync(int id);

        //Task<>
    }
}
//namespace Li_copy.I_InterfaceLayer.FineInterface
//{
//    using Li_copy.Models.Fine;

//    public interface IFineService
//    {
//        Task<IEnumerable<Fine>> GetAllAsync();
//        Task<Fine?> GetByLoanIdAsync(int loanId);
//        Task<int> CreateAsync(Fine fine);
//        Task<bool> UpdateAsync(Fine fine);
//        Task<decimal> CalculateFineAsync(DateTime dueDate, DateTime? returnDate);
//        Task<Fine?> GenerateFineForLoanAsync(int loanId, DateTime dueDate, DateTime? returnDate);
//    }
//}