using Li_copy.Models.Loans;

namespace Li_copy.I_InterfaceLayer.LoanInterface
{
    public interface IloanService
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<Loan?> GetLoanByIdAsync(int id);
        Task<int> CreateLoanAsync(Loan loan);
        Task<bool> UpdateLoanAsync(Loan loan);
        Task<bool> DeleteLoanAsync(int id);
        Task<bool> ReturnBookAsync(int loanId, DateTime returnDate);
    }
}
