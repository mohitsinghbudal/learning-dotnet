using Li_copy.Models.Loans;

namespace Li_copy.I_InterfaceLayer.LoanInterface
{
    public interface IloanDLL
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<Loan?> GetLoanByIdAsync(int id);
        Task<int> CreateLoanAsync(Loan loan);
        Task<bool> UpdateLoanAsync(Loan loan);
        Task<bool> DeleteLoanAsync(int id);
    }
}
